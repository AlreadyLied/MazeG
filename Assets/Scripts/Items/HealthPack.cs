using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, Item
{
    public void Use()
    {
        Player.Instance.Heal(30);
        Destroy(gameObject);
    }
}
