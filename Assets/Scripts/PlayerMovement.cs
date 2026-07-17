using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float kecepatan = 5f;
    private Vector2 arahGerak; // nilai dari action "Move"

    // Dipanggil OTOMATIS oleh komponen Player Input
    // saat action "Move" pada asset InputSystem_Actions aktif.
    // Nama method WAJIB: On + nama action -> OnMove
    void OnMove(InputValue value)
    {
        // TODO: ambil nilai Vector2 dari input, simpan ke arahGerak
        arahGerak = value.Get<Vector2>();
    }
    void Update()
    {
        // TODO: gerakkan objek memakai arahGerak.
        // Ingat kalikan kecepatan DAN Time.deltaTime!
        Vector3 arah = new Vector3(arahGerak.x, arahGerak.y, 0);
        transform.position += arah * kecepatan * Time.deltaTime;
    }

}
