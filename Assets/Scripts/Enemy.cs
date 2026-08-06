using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp = 100;

    public float ms = 2f; //mov speed

    protected Transform player;

    [Header("Pengaturan State Machine")]
    [SerializeField] private float jarakDeteksi = 6f; // masuk CHASE
    [SerializeField] private float jarakSerang = 1.2f; // masuk ATTACK
    [SerializeField] private float jedaSerang = 1f; // detik antar serang

    // state sekarang -- mulai dari IDLE
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    [SerializeField] private float radiusPatrol = 3f;
    private Vector2 titikAwal; // pusat area keliling
    private Vector2 tujuanPatrol; // titik yang sedang dituju


    protected virtual void Start()
    {
        titikAwal = transform.position;
        PilihTujuanPatrolBaru();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // void Update()
    // {
    //     Kejar();
    // }
    void Update()
    {
        // LANGKAH A: tentukan state (aturan pindah)
        PeriksaTransisi();
        // LANGKAH B: jalankan perilaku sesuai state sekarang
        switch (state)
        {
            case StateZombie.IDLE: PerilakuIdle(); break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE: PerilakuChase(); break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    void PerilakuIdle() { }
    // void PerilakuPatrol() { Debug.Log(name + ": PATROL"); }
    // void PerilakuChase() { Debug.Log(name + ": CHASE"); }
    // void PerilakuAttack() { Debug.Log(name + ": ATTACK"); }

    void PeriksaTransisi()
    {
        float jarak = JarakKePlayer(); // sudah ada dari materi OOP!
        if (jarak <= jarakSerang)
            state = StateZombie.ATTACK; // sangat dekat -> serang
        else if (jarak <= jarakDeteksi)
            state = StateZombie.CHASE; // terlihat -> kejar
        else
            state = StateZombie.PATROL; // jauh -> keliling
    }

    void PilihTujuanPatrolBaru()
    {
        Vector2 acak = Random.insideUnitCircle * radiusPatrol;
        tujuanPatrol = titikAwal + acak;
    }

    protected float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    void PerilakuChase() { Kejar(); } // method dari OOP
    void PerilakuAttack()
    {
        // menyerang berkala, tidak tiap frame
        if (Time.time >= waktuSerangTerakhir + jedaSerang)
        {
            Serang(); // method dari OOP
            waktuSerangTerakhir = Time.time;

        }
    }

    void PerilakuPatrol()
    {
        transform.position = Vector2.MoveTowards(
        transform.position, tujuanPatrol, ms * 0.5f * Time.deltaTime);
        if (Vector2.Distance(transform.position, tujuanPatrol) < 0.1f)
            PilihTujuanPatrolBaru();
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
}