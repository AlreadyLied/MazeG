using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    public override void Use()
    {
        Player.Instance.Heal(30);
        Destroy(gameObject);
    }
}
