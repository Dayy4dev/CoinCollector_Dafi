using UnityEngine;
public class GameManager : MonoBehaviour
{
    public int totalKoin;
    private int koinTerkumpul = 0;
    
    void Start()
    {
        // TODO: hitung jumlah koin di scene saat mulai
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        // TODO: jika koinTerkumpul == totalKoin, panggil Menang()
        if (koinTerkumpul == totalKoin) Menang();
    }

    void Menang()
    {
        Debug.Log("<b><color=green>KAMU MENANG!</color></b>");
    }
}