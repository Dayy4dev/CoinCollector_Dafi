using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float kecepatan = 5f;
    private int hp = 100;
    public int skor;
    private GameManager gameManager;
    private Vector2 arahGerak; // nilai dari action "Move"
    private PlayerAnimation animasi;
    private Rigidbody2D rb; // dipakai supaya player bisa nabrak dinding

    // Dipakai oleh PlayerAnimation untuk tahu player sedang gerak ke arah mana
    public Vector2 ArahGerak
    {
        get { return arahGerak; }
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        animasi = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Dipanggil OTOMATIS oleh komponen Player Input
    // saat action "Move" pada asset InputSystem_Actions aktif.
    // Nama method WAJIB: On + nama action -> OnMove
    void OnMove(InputValue value)
    {
        // TODO: ambil nilai Vector2 dari input, simpan ke arahGerak
        arahGerak = value.Get<Vector2>();
    }

    // Pakai FixedUpdate karena geraknya lewat Rigidbody2D (fisika).
    // Kalau pakai transform.position, player akan TEMBUS dinding,
    // karena posisi dipindah paksa tanpa dihitung physics-nya.
    void FixedUpdate()
    {
        if (rb != null)
        {
            // kasih kecepatan ke Rigidbody2D -> physics yang urus tabrakannya
            rb.linearVelocity = arahGerak * kecepatan;
        }
        else
        {
            // cadangan kalau Rigidbody2D lupa dipasang
            Vector3 arah = new Vector3(arahGerak.x, arahGerak.y, 0);
            transform.position += arah * kecepatan * Time.fixedDeltaTime;
        }
    }

    // Dipanggil otomatis saat Player menyentuh objek ber-Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: cek apakah yang disentuh punya tag "Coin"
        if (other.CompareTag("Coin"))
        {
            // TODO: hancurkan koin yang tersentuh
            Destroy(other.gameObject);
            skor += 1;
            Debug.Log("<color=cyan>Skor : </color>" + skor);

            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            } else
            {
                Debug.LogWarning("GameManager tidak ditemukan!");
            }
        }
    }

    public void KenaDamage(int damage)
    {
        // Oper ke PlayerHealth jika ada
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.KenaDamage(damage);
            return;
        }

        if (hp <= 0)
        {
            return;
        }

        hp = Mathf.Max(0, hp - damage);
        Debug.Log("<color=yellow>Player Kena Damage: " + damage + ", sisa HP: " + hp + "</color>");

        // Jalankan animasi hurt
        if (animasi != null)
        {
            animasi.MainHurt();
        }

        if (hp == 0)
        {
            Debug.Log("<color=red>Player Mati</color>");

            // Jalankan animasi mati
            if (animasi != null)
            {
                animasi.MainMati();
            }
        }
    }

}
