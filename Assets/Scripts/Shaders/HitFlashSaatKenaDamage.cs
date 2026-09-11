using UnityEngine;

// PELAJARAN 4 — flash putih saat zombie kena damage.
// Tempel ke zombie. Material Sprite Renderer harus shader Kelas11/Belajar/04_Flash.
// Enemy.KenaDamage akan memanggil MulaiFlash() kalau komponen ini ada.
[RequireComponent(typeof(SpriteRenderer))]
public class HitFlashSaatKenaDamage : MonoBehaviour
{
    static readonly int IdFlashAmount = Shader.PropertyToID("_FlashAmount");

    [SerializeField] float lamaFlash = 0.1f;

    SpriteRenderer spriteRenderer;
    MaterialPropertyBlock blok;
    float sisaFlash;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        blok = new MaterialPropertyBlock();
    }

    public void MulaiFlash()
    {
        sisaFlash = lamaFlash;
    }

    void Update()
    {
        if (sisaFlash > 0f)
        {
            sisaFlash -= Time.deltaTime;
        }

        float jumlah = sisaFlash > 0f ? 1f : 0f;
        spriteRenderer.GetPropertyBlock(blok);
        blok.SetFloat(IdFlashAmount, jumlah);
        spriteRenderer.SetPropertyBlock(blok);
    }
}
