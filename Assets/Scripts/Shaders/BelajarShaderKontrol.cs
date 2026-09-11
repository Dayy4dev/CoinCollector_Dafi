using UnityEngine;
using UnityEngine.InputSystem;

// PELAJARAN shader + C#
// Tempel ke Sprite yang material-nya pakai Kelas11/Belajar/03_Waktu atau 04_Flash.
// Play, tekan Spasi = flash. Slider di Inspector mengatur denyut tanpa meng-instantiate material.
[RequireComponent(typeof(SpriteRenderer))]
public class BelajarShaderKontrol : MonoBehaviour
{
    static readonly int IdKecepatanDenyut = Shader.PropertyToID("_KecepatanDenyut");
    static readonly int IdFlashAmount = Shader.PropertyToID("_FlashAmount");

    [SerializeField] float kecepatanDenyut = 3f;
    [SerializeField] float lamaFlash = 0.12f;

    SpriteRenderer spriteRenderer;
    MaterialPropertyBlock blok;
    float sisaFlash;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        blok = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            sisaFlash = lamaFlash;
        }

        if (sisaFlash > 0f)
        {
            sisaFlash -= Time.deltaTime;
        }

        spriteRenderer.GetPropertyBlock(blok);
        blok.SetFloat(IdKecepatanDenyut, kecepatanDenyut);
        blok.SetFloat(IdFlashAmount, sisaFlash > 0f ? 1f : 0f);
        spriteRenderer.SetPropertyBlock(blok);
    }
}
