using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [Header("Syarat Menang")]
    [Tooltip("Nama item kunci di inventory yang dibutuhkan untuk menang")]
    public string namaKunci = "Key";

    [Header("UI Menang")]
    [Tooltip("Panel menang yang akan diaktifkan saat player berhasil")]
    public GameObject winPanel;

    [Tooltip("Apakah game di-pause (waktu berhenti) saat menang")]
    public bool pauseSaatMenang = true;

    private bool sudahMenang = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jangan jalankan lagi jika sudah menang
        if (sudahMenang) return;

        // 1. Cek apakah objek yang menyentuh collider Finish adalah Player
        if (collision.CompareTag("Player"))
        {
            // 2. Ambil InventoryManager untuk memeriksa item yang dibawa player
            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

            // 3. Cek apakah player membawa kunci yang sesuai
            if (inventory != null && inventory.HasItem(namaKunci))
            {
                TampilkanMenang();
            }
            else
            {
                Debug.Log("<color=yellow>Pintu Finish terkunci! Kamu butuh item '" + namaKunci + "' untuk membuka finish.</color>");
            }
        }
    }

    private void TampilkanMenang()
    {
        sudahMenang = true;

        // Aktifkan panel menang
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Menang();
        }

        // Hentikan permainan agar player dan musuh berhenti bergerak
        if (pauseSaatMenang)
        {
            Time.timeScale = 0f;
        }
    }
}
