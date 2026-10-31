using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalKoin;
    private int koinTerkumpul = 0;
    private int jumlahZombieMati = 0;

    [Header("UI Game Over & Win")]
    [Tooltip("Panel GameOver yang akan muncul saat player kalah / darahnya 0")]
    public GameObject gameOverPanel;

    [Tooltip("Panel Menang yang akan muncul saat player mencapai finish")]
    public GameObject winPanel;

    [Tooltip("Jeda sebelum GameOverPanel muncul agar animasi mati player terlihat (detik)")]
    public float delayGameOver = 0.6f;

    [Tooltip("Apakah game dipause (Time.timeScale = 0) saat panel GameOver muncul")]
    public bool pauseSaatGameOver = true;

    private PlayerHealth playerHealth;
    private bool sudahGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        Enemy.OnZombieMati += SaatZombieMati;
    }

    void OnDisable()
    {
        Enemy.OnZombieMati -= SaatZombieMati;
        if (playerHealth != null)
        {
            playerHealth.OnPlayerMati -= SaatPlayerMati;
        }
    }

    void Start()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;

        // Cari PlayerHealth jika belum terpasang
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnPlayerMati += SaatPlayerMati;
        }

        // Cari GameOverPanel jika belum di-assign di Inspector
        if (gameOverPanel == null)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all)
            {
                if (go.name == "GameOverPanel" && go.scene == gameObject.scene)
                {
                    gameOverPanel = go;
                    break;
                }
            }
        }

        // Cari WinPanel jika belum di-assign di Inspector
        if (winPanel == null)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all)
            {
                if ((go.name == "WinPanel" || go.name == "PanelMenang") && go.scene == gameObject.scene)
                {
                    winPanel = go;
                    break;
                }
            }
        }

        // Pastikan di awal game panel GameOver dan Win tertutup
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void SaatZombieMati(Enemy zombie)
    {
        jumlahZombieMati++;
        Debug.Log("GameManager dengar event. Zombie mati: " + jumlahZombieMati + " (" + zombie.name + ")");
    }

    void SaatPlayerMati()
    {
        Kalah();
    }

    public void Kalah()
    {
        if (sudahGameOver) return;
        sudahGameOver = true;

        Debug.Log("<color=red><b>GAME OVER! Player kalah / darahnya habis.</b></color>");
        StartCoroutine(MunculkanGameOver());
    }

    private IEnumerator MunculkanGameOver()
    {
        if (delayGameOver > 0f)
        {
            yield return new WaitForSecondsRealtime(delayGameOver);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (pauseSaatGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        if (koinTerkumpul == totalKoin)
        {
            Menang();
        }
    }

    public void Menang()
    {
        Debug.Log("<color=green><b>KAMU MENANG!</b></color>");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        if (pauseSaatGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        sudahGameOver = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // void OnGUI()
    // {
    //     GUI.skin.label.fontSize = 22;
    //     // Posisikan di bawah HealthBar (y=135 ke bawah) agar rapi dan tidak tumpang tindih
    //     GUI.Label(new Rect(20, 135, 480, 36), "Koin: " + koinTerkumpul + " / " + totalKoin);
    //     GUI.Label(new Rect(20, 170, 480, 36), "Zombie mati: " + jumlahZombieMati);
    // }
}
