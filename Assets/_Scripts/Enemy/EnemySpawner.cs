using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void BersihkanDaftarMusuh()
    {
        daftarMusuhAktif.RemoveAll(item => item == null);
    }
}
