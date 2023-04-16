using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fairy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 exitPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        exitPos = MapManager.instance.GetExitPos();

        agent.SetDestination(exitPos);
        
        Destroy(gameObject, 10f);
    }
}
