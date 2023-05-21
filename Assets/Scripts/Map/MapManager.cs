using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    private static int[,] maze;
    
    [SerializeField] private MazeConfiguration mazeConfig;
    [SerializeField] private NavMeshSurface surface;

    private void Awake()
    {
        instance = this;
        
        maze = MapGenerator.InitMap(mazeConfig);
        surface.BuildNavMesh();
    }

    public Vector3 GetRandomPos()
    {
        int mapLength = mazeConfig.mapSize * 2 - 1;
        int wallSize = mazeConfig.wallSize;
        while (true)
        {
            int x = UnityEngine.Random.Range(1, mapLength - 1);
            int y = UnityEngine.Random.Range(1, mapLength - 1);
            if (maze[x, y] == 0) return new Vector3(x * wallSize, 0, y * wallSize);
        }
    }

    public Vector3 GetExitPos()
    {
        return MapGenerator.exitPosition;
    }

    public void SpawnMonster()
    {
        int mapSize = mazeConfig.mapSize;
        int wallSize = mazeConfig.wallSize;
        GameObject[] monsterPrefabs = mazeConfig.monsterPrefabs;
        int monsterNum = monsterPrefabs.Length;

        GameObject monster = monsterPrefabs[UnityEngine.Random.Range(0, monsterNum)];
        Vector3 spawnLocation = new Vector3(mapSize * wallSize, 0, mapSize * wallSize + 3);
        Instantiate(monster, spawnLocation, Quaternion.identity);
    }
}