using System;
using UnityEngine;
using UnityEngine.InputSystem;

// PELAJARAN 2 — Event (si pemancar)
// Tempel ke Empty GameObject. Event = delegate yang lebih aman:
// kelas lain boleh langganan (+=), tapi HANYA pemancar yang boleh Invoke.
public class PemancarEvent : MonoBehaviour
{
    public static event Action OnTekanSpasi;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Pemancar: spasi ditekan, kirim event.");
            OnTekanSpasi?.Invoke();
        }
    }
}
