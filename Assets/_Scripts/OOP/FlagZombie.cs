using UnityEngine;

public class FlagZombie : Enemy
{
    public bool flag = true;

    public override void Serang()
    {
        Debug.Log("FlagZombie Gigit");
    }
}
