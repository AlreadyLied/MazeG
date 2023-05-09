using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmDistractor : Item
{
    public GameObject alarmPrefab;
    
    public override void Use()
    {
        Used();
        
        Destroy(gameObject);

        GameObject alarm = Instantiate(alarmPrefab, Player.Position, Quaternion.identity);
    }
}
