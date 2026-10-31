using UnityEngine;
using Unity.Cinemachine;


public class CameraShake : MonoBehaviour
{
    // Singleton sederhana agar mudah dipanggil dari script mana saja:
    // Contoh: CameraShake.Instance.Guncang();
    public static CameraShake Instance { get; private set; }

    [Header("Referensi Komponen")]
    [Tooltip("Komponen Perlin Noise Cinemachine untuk menghasilkan getaran halus dan natural")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin perlin;

    [Tooltip("Profil getaran bawaan Cinemachine (6D Shake)")]
    [SerializeField] private NoiseSettings profilNoise;

    [Header("Pengaturan Hentakan")]
    [Tooltip("Kekuatan guncangan kamera. Angka 1.0 - 1.5 pas untuk hentakan tanpa bikin pusing.")]
    [Range(0.1f, 5f)]
    public float kekuatanGuncang = 1.2f;

    [Tooltip("Durasi getaran dalam detik (0.15 - 0.25 detik ideal untuk hit impact)")]
    [Range(0.05f, 1f)]
    public float durasiGuncang = 0.2f;

    [Tooltip("Frekuensi atau kecepatan getar (makin tinggi makin bergetar cepat)")]
    public float frekuensiGuncang = 2.0f;

    // Variabel pengatur waktu (timer)
    private float sisaWaktu = 0f;
    private float totalDurasi = 0.2f;
    private float intensitasAwal = 1.2f;

    void Awake()
    {
        // Inisialisasi Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        SiapkanKomponen();
    }

    void OnValidate()
    {
        // Membantu otomatis mengisi profil dan komponen di editor Unity
        SiapkanKomponen();
    }

    private void SiapkanKomponen()
    {
        if (perlin == null)
        {
            perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin == null)
            {
                perlin = gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

#if UNITY_EDITOR
        if (profilNoise == null)
        {
            profilNoise = UnityEditor.AssetDatabase.LoadAssetAtPath<NoiseSettings>(
                "Packages/com.unity.cinemachine/Presets/Noise/6D Shake.asset"
            );
        }
#endif

        if (perlin != null)
        {
            if (perlin.NoiseProfile == null && profilNoise != null)
            {
                perlin.NoiseProfile = profilNoise;
            }

            // Saat tidak bermain, pastikan kamera tidak bergetar
            if (!Application.isPlaying)
            {
                perlin.AmplitudeGain = 0f;
            }
        }
    }

    void Start()
    {
        if (perlin != null)
        {
            perlin.AmplitudeGain = 0f;
            perlin.FrequencyGain = frekuensiGuncang;
        }
    }

    void Update()
    {
        // Hitung mundur waktu getar
        if (sisaWaktu > 0f)
        {
            sisaWaktu -= Time.deltaTime;

            if (perlin != null)
            {
                // Menghitung persentase waktu tersisa: 1.0 (baru kena) -> 0.0 (selesai)
                float progres = Mathf.Clamp01(sisaWaktu / totalDurasi);

                // Ease-Out kuadrat (progres * progres):
                // Memberikan hentakan instan di awal (impact punchy),
                // lalu meredam secara lembut sehingga terasa empuk dan nyaman di mata.
                perlin.AmplitudeGain = Mathf.Lerp(0f, intensitasAwal, progres * progres);

                if (sisaWaktu <= 0f)
                {
                    perlin.AmplitudeGain = 0f;
                }
            }
        }
    }

    public void Guncang()
    {
        Guncang(kekuatanGuncang, durasiGuncang);
    }

    public void Guncang(float kekuatan, float durasi)
    {
        if (perlin == null) return;

        intensitasAwal = kekuatan;
        totalDurasi = durasi > 0f ? durasi : 0.01f;
        sisaWaktu = totalDurasi;

        perlin.FrequencyGain = frekuensiGuncang;
        perlin.AmplitudeGain = intensitasAwal;
    }

    [ContextMenu("Tes Guncang Kamera")]
    public void TesGuncangDiInspector()
    {
        Guncang(kekuatanGuncang, durasiGuncang);
    }
}
