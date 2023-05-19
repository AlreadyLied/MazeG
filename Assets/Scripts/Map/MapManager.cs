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

    private void Start()
    {
        Player.Position = (Vector3.forward + Vector3.right) * mazeConfig.mapSize * mazeConfig.wallSize;
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
        
        List<Tuple<int, int>> spawnLocations = new();
        spawnLocations.Add(new Tuple<int, int>(mapSize + 2, mapSize));
        spawnLocations.Add(new Tuple<int, int>(mapSize, mapSize + 2));
        spawnLocations.Add(new Tuple<int, int>(mapSize - 2, mapSize));
        spawnLocations.Add(new Tuple<int, int>(mapSize, mapSize - 2));

        Tuple<int, int> chosenLocation = spawnLocations[UnityEngine.Random.Range(0, 4)];

        GameObject monster = monsterPrefabs[UnityEngine.Random.Range(0, monsterNum)];
        Vector3 spawnLocation = new Vector3(chosenLocation.Item1 * wallSize, 0, chosenLocation.Item2 * wallSize);
        GameObject.Instantiate(monster, spawnLocation, Quaternion.identity);
    }
}