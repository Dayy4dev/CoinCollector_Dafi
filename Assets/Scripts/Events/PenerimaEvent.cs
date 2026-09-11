using UnityEngine;

// PELAJARAN 2 — Event (si penerima)
// Tempel ke GameObject LAIN (boleh Empty). Penerima tidak perlu kenal detail pemancar.
public class PenerimaEvent : MonoBehaviour
{
    void OnEnable()
    {
        PemancarEvent.OnTekanSpasi += Reaksi;
    }

    void OnDisable()
    {
        PemancarEvent.OnTekanSpasi -= Reaksi;
    }

    void Reaksi()
    {
        Debug.Log("Penerima: aku dengar event spasi!");
    }
}
