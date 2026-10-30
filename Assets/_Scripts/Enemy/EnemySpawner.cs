using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy Spawner sederhana untuk memunculkan musuh di titik-titik spawn yang ada di scene.
/// Dibuat ramah pemula:
/// - Ada jeda spawn berkala (Coroutine).
/// - Ada batas maksimal musuh di arena agar game tidak lag atau terlalu ramai.
/// - Otomatis mendeteksi EnemySpawnPoint di scene jika list belum diisi di Inspector.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab Musuh")]
    [Tooltip("Prefab musuh yang akan di-spawn (misal: DragonEnemy)")]
    public GameObject enemyPrefab;

    [Header("Titik-titik Spawn")]
    [Tooltip("Daftar titik spawn. Kalau kosong, otomatis mencari objek berawalan 'EnemySpawnPoint' di scene")]
    public Transform[] titikSpawn;

    [Header("Pengaturan Waktu")]
    [Tooltip("Jeda waktu antara setiap spawn (dalam detik)")]
    public float jedaSpawn = 5f;

    [Tooltip("Batas maksimal musuh aktif bersamaan di scene")]
    public int maksimalMusuh = 3;

    [Tooltip("Apakah langsung spawn musuh di awal game?")]
    public bool spawnDiAwal = true;

    // List untuk mencatat musuh yang sedang hidup di arena
    private List<GameObject> daftarMusuhAktif = new List<GameObject>();

    void Start()
    {
        // 1. Kalau di Inspector belum diisi titikSpawn-nya, cari otomatis di scene
        if (titikSpawn == null || titikSpawn.Length == 0)
        {
            CariTitikSpawnDiScene();
        }

        // 2. Spawn pertama kali di awal permainan jika diaktifkan
        if (spawnDiAwal)
        {
            // Spawn 1 musuh di tiap titik spawn (maksimal sejumlah maksimalMusuh)
            int jumlahAwal = Mathf.Min(titikSpawn.Length, maksimalMusuh);
            for (int i = 0; i < jumlahAwal; i++)
            {
                SpawnMusuhDiTitik(titikSpawn[i]);
            }
        }

        // 3. Jalankan loop spawn otomatis
        StartCoroutine(LoopSpawn());
    }

    /// <summary>
    /// Mencari semua GameObject di scene yang namanya diawali "EnemySpawnPoint"
    /// </summary>
    void CariTitikSpawnDiScene()
    {
        Transform[] semuaTransform = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        List<Transform> hasilCari = new List<Transform>();

        foreach (Transform t in semuaTransform)
        {
            if (t.name.StartsWith("EnemySpawnPoint"))
            {
                hasilCari.Add(t);
            }
        }

        titikSpawn = hasilCari.ToArray();
        Debug.Log("EnemySpawner menemukan " + titikSpawn.Length + " titik spawn di scene.");
    }

    /// <summary>
    /// Coroutine yang jalan terus setiap beberapa detik untuk memunculkan musuh
    /// </summary>
    IEnumerator LoopSpawn()
    {
        while (true)
        {
            // Tunggu selama jedaSpawn detik
            yield return new WaitForSeconds(jedaSpawn);

            // Bersihkan musuh yang sudah mati / destroyed dari list
            BersihkanDaftarMusuh();

            // Cek apakah jumlah musuh masih di bawah batas maksimal
            if (daftarMusuhAktif.Count < maksimalMusuh)
            {
                SpawnSatuMusuhAcak();
            }
        }
    }

    /// <summary>
    /// Memilih satu titik spawn secara acak, lalu membuat musuh di sana
    /// </summary>
    public void SpawnSatuMusuhAcak()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: enemyPrefab belum dipasang!");
            return;
        }

        if (titikSpawn == null || titikSpawn.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: tidak ada titik spawn yang tersedia!");
            return;
        }

        // Pilih index acak dari array titikSpawn
        int indexAcak = Random.Range(0, titikSpawn.Length);
        Transform spawnTerpilih = titikSpawn[indexAcak];

        SpawnMusuhDiTitik(spawnTerpilih);
    }

    void SpawnMusuhDiTitik(Transform targetSpawn)
    {
        if (targetSpawn == null || enemyPrefab == null) return;

        // Buat GameObject musuh baru dari Prefab
        GameObject musuhBaru = Instantiate(enemyPrefab, targetSpawn.position, Quaternion.identity);

        // Beri nama yang rapi
        musuhBaru.name = enemyPrefab.name + " (" + (daftarMusuhAktif.Count + 1) + ")";

        // Masukkan ke list pencatat
        daftarMusuhAktif.Add(musuhBaru);

        Debug.Log("<color=green>Spawned musuh di: " + targetSpawn.name + "</color>");
    }

    /// <summary>
    /// Menghapus referensi null (musuh yang sudah hancur/meledak) dari list
    /// </summary>
    void BersihkanDaftarMusuh()
    {
        daftarMusuhAktif.RemoveAll(item => item == null);
    }

    // Gambar lingkaran hijau di tiap titik spawn untuk mempermudah melihat di editor
    void OnDrawGizmosSelected()
    {
        if (titikSpawn == null) return;

        Gizmos.color = Color.green;
        foreach (Transform t in titikSpawn)
        {
            if (t != null)
            {
                Gizmos.DrawWireSphere(t.position, 0.5f);
            }
        }
    }
}
