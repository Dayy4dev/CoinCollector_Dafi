using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Referensi Script Player")]
    [Tooltip("Target player yang darahnya ingin kita tampilkan di bar")]
    public PlayerHealth playerHealth;

    [Header("Komponen Gambar UI")]
    [Tooltip("Gambar bar MERAH yang akan berkurang saat kena hit")]
    public Image barMerah;

    [Tooltip("Gambar background hitam/kosong (wadah bar)")]
    public Image barKosong;

    [Tooltip("Komponen Animator pada icon Jantung berdenyut")]
    public Animator heartAnimator;

    [Header("Teks Angka HP (Opsional)")]
    [Tooltip("Teks angka HP (contoh: 100 / 100). Boleh dikosongkan kalau tidak mau pakai teks.")]
    public TextMeshProUGUI teksAngkaHP;

    [Header("Pengaturan Efek")]
    [Tooltip("Kecepatan bar merah menyusut. Makin besar angkanya, makin cepat susutnya.")]
    public float kecepatanAnimasiBar = 6f;

    [Tooltip("Batas HP dianggap kritis (0.3 = 30%)")]
    [Range(0f, 1f)]
    public float batasKritis = 0.3f;

    // Nilai target pengisian (0.0 sampai 1.0)
    private float targetFill = 1f;

    void Awake()
    {
        // Kalau lupa pasang playerHealth di Inspector, cari otomatis
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
    }

    void OnEnable()
    {
        // Mulai mendengarkan perubahan HP dari Player
        if (playerHealth != null)
        {
            playerHealth.OnHPBerubah += PerbaruiTampilan;
            playerHealth.OnPlayerMati += SaatPlayerMati;

            // Langsung samakan tampilan awal
            PerbaruiTampilan(playerHealth.hpSekarang, playerHealth.maxHP);
        }
    }

    void OnDisable()
    {
        // Berhenti mendengarkan saat UI dimatikan (penting agar tidak bocor memory)
        if (playerHealth != null)
        {
            playerHealth.OnHPBerubah -= PerbaruiTampilan;
            playerHealth.OnPlayerMati -= SaatPlayerMati;
        }
    }

    void Start()
    {
        // Pastikan barMerah diset ke tipe 'Filled' secara otomatis
        // supaya orang baru belajar tidak bingung kenapa gambarnya tidak menyusut!
        if (barMerah != null)
        {
            barMerah.type = Image.Type.Filled;
            barMerah.fillMethod = Image.FillMethod.Horizontal;
            barMerah.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        if (playerHealth != null)
        {
            PerbaruiTampilan(playerHealth.hpSekarang, playerHealth.maxHP);
        }
    }

    void Update()
    {
        // Buat bar merah menyusut secara HALUS (lerp)
        // Jadi ada efek animasi turunnya, tidak langsung patah/patah
        if (barMerah != null)
        {
            barMerah.fillAmount = Mathf.Lerp(barMerah.fillAmount, targetFill, Time.deltaTime * kecepatanAnimasiBar);
        }
    }

    public void PerbaruiTampilan(int hpSekarang, int maxHP)
    {
        if (maxHP <= 0) return;

        // Hitung persentase (0.0f sampai 1.0f)
        targetFill = Mathf.Clamp01((float)hpSekarang / maxHP);

        // Update teks angka (kalau ada teksnya)
        if (teksAngkaHP != null)
        {
            teksAngkaHP.text = hpSekarang + " / " + maxHP;
        }

        // Atur animasi detak jantung
        if (heartAnimator != null)
        {
            if (hpSekarang <= 0)
            {
                // Mati: jantung berhenti
                heartAnimator.speed = 0f;
            }
            else if (targetFill <= batasKritis)
            {
                // Kritis: jantung berdetak kencang/panik!
                heartAnimator.speed = 2.0f;
            }
            else
            {
                // Normal: detak santai
                heartAnimator.speed = 1.0f;
            }
        }
    }

    void SaatPlayerMati()
    {
        targetFill = 0f;
        if (heartAnimator != null)
        {
            heartAnimator.speed = 0f; // detak berhenti
        }
    }
}
