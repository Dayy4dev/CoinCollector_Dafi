using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragonEnemy : MonoBehaviour, IDamageable
{
    [Header("Gerak")]
    [Tooltip("Kecepatan terbang saat patroli (unit per detik)")]
    public float kecepatanPatroli = 1.5f;

    [Tooltip("Kecepatan terbang saat mengejar player")]
    public float kecepatanKejar = 3.5f;

    [Tooltip("Seberapa jauh naga berkeliling dari titik spawn-nya")]
    public float radiusPatroli = 2.8f;

    [Tooltip("Diam sebentar dulu sebelum terbang ke titik patroli berikutnya")]
    public float jedaPatroli = 0.7f;

    [Tooltip("Radius badan untuk cek tabrakan. Samakan dengan CircleCollider2D")]
    public float radiusBadan = 0.32f;

    [Header("Deteksi Player")]
    [Tooltip("Player masuk radius ini -> naga mulai mengejar")]
    public float jarakDeteksi = 6f;

    [Tooltip("Player sedekat ini -> naga langsung meledak")]
    public float jarakLedak = 1.1f;

    [Header("Ledakan (serangan)")]
    [Tooltip("Damage yang diterima player kalau kena ledakan")]
    public int damageLedak = 25;

    [Tooltip("Radius ledakan (semua IDamageable di dalam radius ini kena damage)")]
    public float radiusLedak = 1.8f;

    [Tooltip("Lama animasi ledakan diputar sebelum object dihancurkan")]
    public float lamaAnimasiLedak = 0.5f;

    [Header("Darah")]
    public int hp = 30;

    [Header("Referensi komponen (otomatis diisi kalau kosong)")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    // ---- State machine ----
    private StateZombie state = StateZombie.PATROL;

    // ---- Patroli ----
    private Vector2 posisiAwal;      // titik spawn, jadi pusat area patroli
    private Vector2 titikPatroli;    // tujuan terbang saat ini
    private float sisaJeda;          // > 0 artinya naga sedang diam sebentar

    // ---- Mengejar ----
    private Transform player;

    // ---- Menghindari dinding ----
    private Vector2 arahHindar;      // arah memutar yang sedang dipakai
    private float sisaWaktuHindar;
    private float waktuBuntu;        // berapa lama jalur kita tertutup terus

    // ---- Status ----
    private bool sudahMeledak;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        CariPlayer();

        posisiAwal = transform.position;
        PilihTitikPatroliBaru();

        // mulai dari animasi terbang
        if (animator != null) animator.SetBool("Mengejar", false);
    }

    void CariPlayer()
    {
        GameObject objekPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objekPlayer != null) player = objekPlayer.transform;
    }

    void Update()
    {
        if (sudahMeledak) return;

        if (player == null) CariPlayer();

        float jarak = JarakKePlayer();

        // 1) Sudah dekat sekali? Langsung meledak.
        if (jarak <= jarakLedak)
        {
            MulaiLedak();
            return;
        }

        // 2) Tentukan state dari jarak player
        state = (jarak <= jarakDeteksi) ? StateZombie.CHASE : StateZombie.PATROL;

        // 3) Kabari Animator supaya pindah animasi (terbang <-> meluncur)
        if (animator != null) animator.SetBool("Mengejar", state == StateZombie.CHASE);

        // 4) Kalau sedang patroli, atur titik tujuan berikutnya
        if (state == StateZombie.PATROL) PerbaruiPatroli();
    }

    void PerbaruiPatroli()
    {
        if (sisaJeda > 0f)
        {
            // sedang diam sebentar di titik patroli
            sisaJeda -= Time.deltaTime;
            if (sisaJeda <= 0f) PilihTitikPatroliBaru();
            return;
        }

        // sudah sampai di titik tujuan? diam dulu, lalu cari titik baru
        if (Vector2.Distance(transform.position, titikPatroli) < 0.25f)
        {
            sisaJeda = jedaPatroli;
        }
    }

    /// <summary>
    /// Memilih titik patroli acak di sekitar titik spawn, TAPI hanya titik yang
    /// jalurnya benar-benar kosong. Kalau 20 kali percobaan semuanya kena dinding,
    /// naga memilih diam dulu (lebih baik daripada nabrak tembok).
    /// </summary>
    void PilihTitikPatroliBaru()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 calon = posisiAwal + Random.insideUnitCircle * radiusPatroli;

            if (JalurBebas(transform.position, calon))
            {
                titikPatroli = calon;
                waktuBuntu = 0f;
                return;
            }
        }

        // gagal semua -> tetap di tempat sebentar
        titikPatroli = transform.position;
        sisaJeda = jedaPatroli;
    }

    // =====================================================================
    // FIXED UPDATE: semua gerakan lewat Rigidbody2D supaya tidak tembus dinding
    // =====================================================================
    void FixedUpdate()
    {
        if (sudahMeledak)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // sedang jeda patroli? diam.
        if (state == StateZombie.PATROL && sisaJeda > 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 tujuan = (state == StateZombie.CHASE && player != null)
            ? (Vector2)player.position
            : titikPatroli;

        float kecepatan = (state == StateZombie.CHASE) ? kecepatanKejar : kecepatanPatroli;

        Vector2 selisih = tujuan - (Vector2)transform.position;
        if (selisih.magnitude < 0.15f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 arahIdeal = selisih.normalized;

        // cari arah yang benar-benar bisa dilewati (tidak nabrak dinding)
        Vector2 arahAman = CariArahAman(arahIdeal);

        // pelan-pelan saat mau sampai, biar tidak lewat terus bolak-balik
        if (selisih.magnitude < 0.8f) kecepatan *= 0.6f;

        rb.linearVelocity = arahAman * kecepatan;

        // hadapkan sprite ke arah gerak (gambar aslinya menghadap kanan)
        if (Mathf.Abs(rb.linearVelocity.x) > 0.05f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0f;
        }

        // kalau arah ideal buntu terus selama 1 detik, cari titik patroli baru
        if (state == StateZombie.PATROL && waktuBuntu > 1f)
        {
            PilihTitikPatroliBaru();
            waktuBuntu = 0f;
        }
    }

    /// <summary>
    /// Mengembalikan arah yang aman untuk dilalui.
    /// Kalau arah lurus terhalang dinding, arah diputar 30, 60, 90, ... derajat
    /// ke kiri dan ke kanan sampai ketemu celah. Ini yang membuat naga
    /// "menyusuri" dinding bukan menabraknya terus.
    /// </summary>
    Vector2 CariArahAman(Vector2 arahIdeal)
    {
        float jarakCek = 0.7f;

        // 1) masih dalam mode menghindar? pakai arah itu selama masih aman
        if (sisaWaktuHindar > 0f)
        {
            sisaWaktuHindar -= Time.fixedDeltaTime;

            if (BisaLewat(arahHindar, jarakCek))
            {
                waktuBuntu = 0f;
                return arahHindar;
            }
            sisaWaktuHindar = 0f;
        }

        // 2) arah lurus masih kosong? langsung pakai
        if (BisaLewat(arahIdeal, jarakCek))
        {
            waktuBuntu = 0f;
            return arahIdeal;
        }

        // 3) terhalang -> cari celah dengan memutar arah
        for (int langkah = 1; langkah <= 6; langkah++)
        {
            float sudut = langkah * 30f;

            Vector2 keKiri = PutarArah(arahIdeal, sudut);
            if (BisaLewat(keKiri, jarakCek))
            {
                arahHindar = keKiri;
                sisaWaktuHindar = 0.45f;
                waktuBuntu = 0f;
                return keKiri;
            }

            Vector2 keKanan = PutarArah(arahIdeal, -sudut);
            if (BisaLewat(keKanan, jarakCek))
            {
                arahHindar = keKanan;
                sisaWaktuHindar = 0.45f;
                waktuBuntu = 0f;
                return keKanan;
            }
        }

        // 4) buntu di semua arah (misal di pojok) -> hitung sebagai buntu
        waktuBuntu += Time.fixedDeltaTime;
        return arahIdeal;
    }

    /// <summary>
    /// Cek apakah naga bisa lewat ke arah tertentu tanpa nabrak dinding/props.
    /// Pakai CircleCast (bukan Linecast) supaya lebar badan naga ikut dihitung.
    /// </summary>
    bool BisaLewat(Vector2 arah, float jarak)
    {
        RaycastHit2D[] kena = Physics2D.CircleCastAll(transform.position, radiusBadan, arah, jarak);

        foreach (RaycastHit2D h in kena)
        {
            if (h.collider == null) continue;
            if (h.collider.gameObject == gameObject) continue;  // collider sendiri
            if (h.collider.isTrigger) continue;                 // koin, item, dll
            if (h.collider.CompareTag("Player")) continue;      // player bukan penghalang
            if (h.distance < 0.05f) continue;                   // sedang nempel, biarkan lepas

            return false;
        }
        return true;
    }

    /// <summary>
    /// Cek apakah jalur lurus dari titik A ke titik B kosong (dipakai saat memilih titik patroli).
    /// </summary>
    bool JalurBebas(Vector2 dari, Vector2 ke)
    {
        Vector2 selisih = ke - dari;
        float jarak = selisih.magnitude;
        if (jarak < 0.2f) return false;

        RaycastHit2D[] kena = Physics2D.CircleCastAll(dari, radiusBadan, selisih.normalized, jarak);

        foreach (RaycastHit2D h in kena)
        {
            if (h.collider == null) continue;
            if (h.collider.gameObject == gameObject) continue;
            if (h.collider.isTrigger) continue;
            if (h.collider.CompareTag("Player")) continue;
            if (h.distance < 0.05f) continue;

            return false;
        }
        return true;
    }

    Vector2 PutarArah(Vector2 v, float derajat)
    {
        float rad = derajat * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    // =====================================================================
    // SERANGAN: MELEDAK
    // =====================================================================
    /// <summary>
    /// Dipanggil otomatis saat player sudah terlalu dekat,
    /// atau saat HP habis (kena serangan player).
    /// </summary>
    public void MulaiLedak(bool beriDamage = true)
    {
        if (sudahMeledak) return;

        sudahMeledak = true;
        state = StateZombie.ATTACK;

        // berhenti total supaya ledakannya di tempat
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // matikan collider supaya tidak mendorong player saat meledak
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetTrigger("Meledak");

        StartCoroutine(ProsesLedakan(beriDamage));
    }

    IEnumerator ProsesLedakan(bool beriDamage)
    {
        // tunggu animasi ledakan selesai
        yield return new WaitForSeconds(lamaAnimasiLedak);

        if (beriDamage) Ledakkan();

        // beri sedikit waktu untuk frame terakhir animasi terlihat
        yield return new WaitForSeconds(0.15f);

        Destroy(gameObject);
    }

    void Ledakkan()
    {
        // cari semua collider di dalam radius ledakan
        Collider2D[] kenaRadius = Physics2D.OverlapCircleAll(transform.position, radiusLedak);

        foreach (Collider2D col in kenaRadius)
        {
            // jangan damage diri sendiri
            if (col.gameObject == gameObject) continue;

            IDamageable target = col.GetComponent<IDamageable>();
            if (target != null)
            {
                target.KenaDamage(damageLedak);
            }
        }

        Debug.Log("<color=orange>BOOM! " + name + " meledak.</color>");
    }

    // =====================================================================
    // KENA DAMAGE (dari player)
    // =====================================================================
    public void KenaDamage(int jumlah)
    {
        if (sudahMeledak) return;

        hp -= jumlah;
        Debug.Log("<color=#ff8080>" + name + " kena " + jumlah + " damage, sisa HP: " + hp + "</color>");

        if (hp <= 0)
        {
            // HP habis -> tetap meledak (itu satu-satunya animasi mati),
            // tapi tidak memberi damage ke player
            MulaiLedak(false);
            return;
        }

        // masih hidup -> mainkan animasi hurt (kedip putih)
        if (animator != null) animator.SetTrigger("Hurt");
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    // Biar kelihatan di Scene view:
    // kuning = jarak deteksi, merah = radius ledakan, biru = area patroli + tujuan sekarang
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jarakDeteksi);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusLedak);

        Vector3 pusat = Application.isPlaying ? (Vector3)posisiAwal : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pusat, radiusPatroli);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, titikPatroli);
            Gizmos.DrawWireSphere(titikPatroli, 0.15f);
        }
    }
}
