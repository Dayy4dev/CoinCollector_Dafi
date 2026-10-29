using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerMovement playerMovement;

    bool sudahMati = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        // awal game menghadap bawah
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1);
    }

    void Update()
    {
        if (sudahMati == true)
        {
            return;
        }

        Vector2 arah = playerMovement.ArahGerak;

        // cek apakah player sedang jalan
        if (arah.magnitude > 0.1f)
        {
            animator.SetBool("IsMoving", true);

            float x = 0;
            float y = 0;

            // tentukan kiri/kanan
            if (arah.x > 0)
            {
                x = 1;
            }
            else if (arah.x < 0)
            {
                x = -1;
            }

            // tentukan atas/bawah
            if (arah.y > 0)
            {
                y = 1;
            }
            else
            {
                y = -1;
            }

            // kalau hampir lurus atas/bawah, kiri/kanan diabaikan
            if (Mathf.Abs(arah.x) < Mathf.Abs(arah.y) * 0.2f)
            {
                x = 0;
            }

            // kalau hampir lurus kiri/kanan, pakai animasi bawah
            if (Mathf.Abs(arah.y) < Mathf.Abs(arah.x) * 0.2f)
            {
                y = -1;
            }

            animator.SetFloat("MoveX", x);
            animator.SetFloat("MoveY", y);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    // dipanggil saat kena damage
    public void MainHurt()
    {
        if (sudahMati == true)
        {
            return;
        }

        spriteRenderer.color = Color.red;
        Invoke("WarnaNormal", 0.12f); // kembali normal setelah 0.12 detik
    }

    void WarnaNormal()
    {
        spriteRenderer.color = Color.white;
    }

    // dipanggil saat HP habis
    public void MainMati()
    {
        sudahMati = true;
        animator.SetBool("IsDead", true);
    }
}