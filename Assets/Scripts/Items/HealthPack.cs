using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    public override void Use()
    {
        Player.Health.Heal(30);
        Destroy(gameObject);
    }
}
