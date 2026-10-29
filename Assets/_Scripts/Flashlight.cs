using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public PlayerMovement player;
    public float koreksiSudut = -90f;
    public float kecepatanPutar = 540f;

    private float sudutSekarang;

    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>();
        }

        sudutSekarang = transform.eulerAngles.z;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector2 arah = player.ArahGerak;

        if (arah == Vector2.zero)
        {
            return;
        }

        float sudutTujuan = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg + koreksiSudut;

        sudutSekarang = Mathf.MoveTowardsAngle(
            sudutSekarang,
            sudutTujuan,
            kecepatanPutar * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0, 0, sudutSekarang);
    }
}
