using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingFairy : Item
{
    public GameObject fairyPrefab;

    public override void Use()
    {
        Used();
        Destroy(gameObject);
        
        GameObject fairy = Instantiate(fairyPrefab, Player.Position, Quaternion.identity);
    }
}
