using UnityEngine;
using System;

public class Enemy2 : MonoBehaviour
{
    [SerializeField] private int hp = 100;
    public float ms = 2f;

    protected Transform player;

    [Header("State Machine")]
    [SerializeField] private float JarakDeteksi = 6f;
    [SerializeField] private float JarakSerang = 1.5f;
    [SerializeField] private float JedaSerang = 1f;

    // State Now
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;


    // PELAJARAN 3 — event di game: zombie yang mati memberitahu siapa pun yang sedang langganan.
    // Action<Enemy> = delegate siap pakai, parameter-nya adalah zombie yang mati.
    public static event Action<Enemy> OnZombieMati;

    [SerializeField] private int damageSaatTabrakan = 10;

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        PeriksaTransisi();
        
        switch(state)
        {
            case StateZombie.IDLE: PerilakuIdle(); break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE: PerilakuChase(); break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity   ;
        return Vector2.Distance(transform.position, player.position);
    }

    void PeriksaTransisi()
    {
        float jarak = JarakKePlayer();

        if (jarak <= JarakSerang)
        {
            state = StateZombie.ATTACK;
        }
        else if (jarak <= JarakDeteksi)
        {
            state = StateZombie.CHASE;
        }
        else
        {
            state = StateZombie.PATROL;
        }  
    }

    public void Kejar()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            ms * Time.deltaTime
        );
    }

    public virtual void Serang()
    {
        Debug.Log("Enemy Serang");
    }

    // Dipanggil otomatis oleh Unity saat collider enemy menabrak collider lain.
    // Karena ada di class induk, SEMUA turunan zombie ikut punya perilaku ini.
    void PerilakuIdle()
    {
        Debug.Log("Enemy sedang IDLE");
    }

    void PerilakuPatrol()
    {
        Debug.Log("Enemy sedang PATROL");
    }

    void PerilakuChase()
    {
        Kejar();
        Debug.Log("Enemy sedang CHASE");
    }

    void PerilakuAttack()
    {
        Debug.Log("Enemy sedang ATTACK");
    }

    public void KenaDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Enemy kena damage: " + damage + ", HP sekarang: " + hp);

        // PELAJARAN shader: kalau zombie punya HitFlashSaatKenaDamage, GPU yang me-flash putih.
        HitFlashSaatKenaDamage flash = GetComponent<HitFlashSaatKenaDamage>();
        if (flash != null)
        {
            flash.MulaiFlash();
        }
        
        if (hp <= 0)
        {
            Mati();
        }
    }

    protected virtual void Mati()
    {
        Debug.Log("Enemy mati!");
        OnZombieMati?.Invoke(this);
        Destroy(gameObject);
    }

    [ContextMenu("Tes Event: Zombie Mati")]
    void TesEventMati()
    {
        Mati();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable playerScript = other.GetComponent<IDamageable>();
            if (playerScript != null)
            {
                playerScript.KenaDamage(damageSaatTabrakan);
            }
        }
    }
}
