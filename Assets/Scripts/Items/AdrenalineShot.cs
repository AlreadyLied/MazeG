using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineShot : Item
{
    public override void Use()
    {
        Player.Instance.Bleed(2, 5);
        Player.Movement.SpeedUp(2f, 10);
        Destroy(gameObject);
    }
}
