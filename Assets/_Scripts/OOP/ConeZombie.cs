using UnityEngine;

public class ConeZombie : Enemy
{
    public bool cone = true;

    public override void Serang()
    {
        Debug.Log("Cone Gigit");
    }
}
