using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Pengaturan HP")]
    [Tooltip("HP maksimal player")]
    public int maxHP = 100;

    [Tooltip("HP saat game baru dimulai")]
    public int hpSekarang = 100;

    [Header("Efek Saat Kena Damage")]
    [Tooltip("Setelah kena damage, player kebal sebentar. " +
             "Gunanya supaya 1 serangan musuh tidak mengurangi HP berkali-kali.")]
    public float waktuKebal = 0.6f;

    [Tooltip("Apakah kamera ikut berguncang saat terkena damage agar terasa impact dan hentakannya.")]
    public bool aktifkanCameraShake = true;

    [Tooltip("Apakah layar berkedip merah (blood flash) saat terkena damage seolah darah berkurang.")]
    public bool aktifkanFlashMerah = true;

    [Header("Tombol Debug (untuk ngetes saja)")]
    [Tooltip("H = kena damage, J = tambah HP. Boleh dimatikan kalau sudah selesai belajar.")]
    public bool aktifkanTombolDebug = true;
    public int besarDamageDebug = 10;

    public event Action<int, int> OnHPBerubah;  // (hpSekarang, maxHP)
    public event Action OnPlayerMati;

    public bool SudahMati { get; private set; }

    // Persentase HP (0 sampai 1). Dipakai HealthBarUI untuk mengisi bar.
    public float PersenHP
    {
        get
        {
            if (maxHP <= 0) return 0f;
            return (float)hpSekarang / maxHP;
        }
    }

    private float sisaWaktuKebal = 0f;
    private PlayerAnimation animasi;
    private PlayerMovement gerak;

    void Awake()
    {
        animasi = GetComponent<PlayerAnimation>();
        gerak = GetComponent<PlayerMovement>();

        // jaga-jaga kalau di Inspector angkanya kebablasan
        hpSekarang = Mathf.Clamp(hpSekarang, 0, maxHP);
    }

    void Start()
    {
        // Umumkan HP pertama kali supaya bar langsung terisi penuh
        UmumkanHP();
    }

    void Update()
    {
        // hitung mundur waktu kebal
        if (sisaWaktuKebal > 0f)
        {
            sisaWaktuKebal -= Time.deltaTime;
        }

        // --- Tombol debug, hapus/dimatikan kalau tidak perlu ---
        if (aktifkanTombolDebug && Keyboard.current != null && !SudahMati)
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                KenaDamage(besarDamageDebug);
            }
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                Sembuhkan(besarDamageDebug);
            }
        }
    }

    public void KenaDamage(int damage)
    {
        if (SudahMati) return;

        // masih kebal? damage diabaikan
        if (sisaWaktuKebal > 0f) return;

        hpSekarang = Mathf.Max(0, hpSekarang - damage);
        sisaWaktuKebal = waktuKebal;

        Debug.Log("<color=yellow>Player kena " + damage + " damage. Sisa HP: " + hpSekarang + "</color>");

        // efek player berkedip merah
        if (animasi != null) animasi.MainHurt();

        // efek guncangan kamera (camera shake) agar terasa impact-nya
        if (aktifkanCameraShake)
        {
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Guncang();
            }
            else
            {
                var shake = FindFirstObjectByType<CameraShake>();
                if (shake != null) shake.Guncang();
            }
        }

        // efek flash merah di layar agar terasa darahnya berkurang
        if (aktifkanFlashMerah)
        {
            if (DamageFlashUI.Instance != null)
            {
                DamageFlashUI.Instance.Flash();
            }
            else
            {
                var flash = FindFirstObjectByType<DamageFlashUI>();
                if (flash != null) flash.Flash();
            }
        }

        // kabari HealthBarUI
        UmumkanHP();

        if (hpSekarang <= 0)
        {
            Mati();
        }
    }

    public void Sembuhkan(int jumlah)
    {
        if (SudahMati) return;

        hpSekarang = Mathf.Min(maxHP, hpSekarang + jumlah);
        Debug.Log("<color=green>Player disembuhkan " + jumlah + ". HP: " + hpSekarang + "</color>");

        UmumkanHP();
    }

    void Mati()
    {
        SudahMati = true;

        Debug.Log("<color=red><b>PLAYER MATI</b></color>");

        // jalankan animasi mati
        if (animasi != null) animasi.MainMati();

        // stop player supaya tidak bisa jalan lagi
        if (gerak != null) gerak.enabled = false;

        if (OnPlayerMati != null) OnPlayerMati.Invoke();
        if (GameManager.Instance != null) GameManager.Instance.Kalah();
    }

    void UmumkanHP()
    {
        if (OnHPBerubah != null) OnHPBerubah.Invoke(hpSekarang, maxHP);
    }
}
