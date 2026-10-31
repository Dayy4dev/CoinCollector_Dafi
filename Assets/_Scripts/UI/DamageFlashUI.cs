using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class DamageFlashUI : MonoBehaviour
{
    public static DamageFlashUI Instance { get; private set; }

    [Header("Referensi UI & Player")]
    [Tooltip("Gambar layar merah. Otomatis diambil jika komponen ini ada di GameObject yang sama.")]
    public Image gambarFlash;

    [Tooltip("Referensi PlayerHealth untuk mendeteksi kondisi darah kritis (opsional)")]
    public PlayerHealth playerHealth;

    [Header("Pengaturan Warna & Hentakan")]
    [Tooltip("Warna kilatan darah saat terkena damage (merah pekat / blood red)")]
    public Color warnaFlash = new Color(0.85f, 0.08f, 0.08f, 1f);

    [Tooltip("Tingkat kepekatan maksimal flash (0.0 = transparan, 1.0 = merah pekat)")]
    [Range(0.1f, 1f)]
    public float alphaMaksimal = 0.65f;

    [Tooltip("Kecepatan warna merah memudar kembali bening (makin besar = makin cepat pudar)")]
    public float kecepatanPudar = 3.0f;

    [Header("Efek Denyut Saat Darah Kritis")]
    [Tooltip("Apakah layar berdenyut merah pelan saat HP kritis (< 30%)?")]
    public bool aktifkanDenyutSaatSekarat = true;

    [Tooltip("Batas persentase HP dianggap sekarat (0.3 = 30%)")]
    [Range(0.1f, 0.5f)]
    public float batasSekarat = 0.3f;

    // Nilai alpha saat ini (0.0 sampai 1.0)
    private float alphaSekarang = 0f;

    void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ambil komponen Image otomatis
        if (gambarFlash == null)
        {
            gambarFlash = GetComponent<Image>();
        }

        // Cari PlayerHealth otomatis jika belum dipasang di Inspector
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (gambarFlash != null)
        {
            gambarFlash.raycastTarget = false;
            gambarFlash.color = new Color(warnaFlash.r, warnaFlash.g, warnaFlash.b, 0f);
        }
    }

    void Update()
    {
        if (gambarFlash == null) return;

        // 1. Jika baru terkena hit, pudarkan warna merah secara perlahan
        if (alphaSekarang > 0f)
        {
            alphaSekarang = Mathf.MoveTowards(alphaSekarang, 0f, Time.deltaTime * kecepatanPudar);
        }

        // 2. Efek denyut jantung merah saat darah sekarat/kritis
        float alphaDenyut = 0f;
        if (aktifkanDenyutSaatSekarat && playerHealth != null && !playerHealth.SudahMati)
        {
            if (playerHealth.PersenHP <= batasSekarat && playerHealth.PersenHP > 0f)
            {
                // Gunakan fungsi Sinus (Mathf.Sin) untuk menghasilkan gerakan naik-turun halus (denyut)
                float gelombangSin = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f; // rentang 0.0 - 1.0
                alphaDenyut = Mathf.Lerp(0.08f, 0.28f, gelombangSin);
            }
        }

        // Ambil nilai alpha tertinggi antara hentakan hit atau denyut sekarat
        float alphaAkhir = Mathf.Max(alphaSekarang, alphaDenyut);

        // Pasang warna akhir ke komponen Image
        gambarFlash.color = new Color(warnaFlash.r, warnaFlash.g, warnaFlash.b, alphaAkhir);
    }

    public void Flash()
    {
        Flash(1.0f);
    }

    public void Flash(float pengaliIntensitas)
    {
        // Langsung naikkan alpha seketika agar ada efek 'HENTAKAN' kaget!
        alphaSekarang = Mathf.Clamp01(alphaMaksimal * pengaliIntensitas);

        if (gambarFlash != null)
        {
            gambarFlash.color = new Color(warnaFlash.r, warnaFlash.g, warnaFlash.b, alphaSekarang);
        }
    }

    public void TesFlashDiInspector()
    {
        Flash();
    }
}
