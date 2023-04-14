using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineShot : MonoBehaviour
{
    public void Use()
    {
        // Player.Instance.Bleed(2, 5);
        // Player.Instance.SpeedUp(2f, 10);
        Destroy(gameObject);
    }
}
