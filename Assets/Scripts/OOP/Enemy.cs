using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 100;
    public float ms = 2f;
    [SerializeField] private int damageSaatTabrakan = 20;

    protected Transform player;

    [Header("State Machine")]
    [SerializeField] private float JarakDeteksi = 6f;
    [SerializeField] private float JarakSerang = 1.5f;
    [SerializeField] private float JedaSerang = 1f;
    [SerializeField] private float radiusPatrol = 3f;
    private Vector2 targetPatrol;
    private Vector2 awalPatrol;
    

    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;


    public static event Action<Enemy> OnZombieMati;
    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        awalPatrol = transform.position;
        targetPatrol = awalPatrol + UnityEngine.Random.insideUnitCircle * radiusPatrol;
    }

    void Update()
    {
        PeriksaTransisi();

        switch (state)
        {
            case StateZombie.IDLE: PerilakuIdle(); break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE: PerilakuChase(); break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;
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
        Debug.Log("Enemy menyerang!");
    }

    // Dipanggil otomatis oleh Unity saat collider enemy menabrak collider lain.
    // Karena ada di class induk, SEMUA turunan zombie ikut punya perilaku ini.
    void PerilakuIdle()
    {
        Debug.Log("Enemy sedang IDLE");
    }

    void PerilakuPatrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPatrol,
            ms * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPatrol) < 0.1f)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * radiusPatrol;
            targetPatrol = awalPatrol + randomOffset;
        }
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

        if (hp <= 0)
        {
            Mati();
        }
    }

    // protected virtual void Mati()
    // {
    //     Debug.Log("Enemy mati!");
    //     Destroy(gameObject);
    // }

    protected virtual void Mati()
    {
        Debug.Log(name + " kalah!");

        // '?.Invoke' -> aman walau belum ada yang mendengarkan (tidak error)
        OnZombieMati?.Invoke(this);   // kirim 'this' = info diri sendiri

        Destroy(gameObject);
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
