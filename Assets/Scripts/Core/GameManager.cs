// using UnityEngine;
// public class GameManager : MonoBehaviour
// {
//     public int totalKoin;
//     private int koinTerkumpul = 0;

//     [SerializeField] private int skor = 0;

//     public int Skor => skor;
    
//     void Start()
//     {
//         // TODO: hitung jumlah koin di scene saat mulai
//         totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
//     }

//     public void AmbilKoin()
//     {
//         koinTerkumpul++;
//         // TODO: jika koinTerkumpul == totalKoin, panggil Menang()
//         if (koinTerkumpul == totalKoin) Menang();
//     }

//     void Menang()
//     {
//         Debug.Log("<b><color=green>KAMU MENANG!</color></b>");
//     }

//     void OnEnable()
//     {
//         Enemy.OnZombieMati += TambahSkorSaatZombieMati;
//     }
 
//     void OnDisable()
//     {
//         Enemy.OnZombieMati -= TambahSkorSaatZombieMati;
//     }
 
//     void TambahSkorSaatZombieMati(Enemy zombieYangMati)
//     {
//         skor += 10;
//         Debug.Log("Skor: " + skor);
//     }

// }

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalKoin;
    private int koinTerkumpul = 0;
    private int jumlahZombieMati = 0;

    void OnEnable()
    {
        Enemy.OnZombieMati += SaatZombieMati;
    }

    void OnDisable()
    {
        Enemy.OnZombieMati -= SaatZombieMati;
    }

    void Start()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    void SaatZombieMati(Enemy zombie)
    {
        jumlahZombieMati++;
        Debug.Log("GameManager dengar event. Zombie mati: " + jumlahZombieMati + " (" + zombie.name + ")");
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        if (koinTerkumpul == totalKoin)
        {
            Menang();
        }
    }

    void Menang()
    {
        Debug.Log("KAMU MENANG!");
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 22;
        GUI.Label(new Rect(16, 16, 480, 36), "Koin: " + koinTerkumpul + " / " + totalKoin);
        GUI.Label(new Rect(16, 52, 480, 36), "Zombie mati: " + jumlahZombieMati);
    }
}
