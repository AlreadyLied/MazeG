using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    private static int[,] maze;
    public Camera cam;
    
    [SerializeField] private MazeConfiguration mazeConfig;
    [SerializeField] private NavMeshSurface surface;

    private void Awake()
    {
        instance = this;
        
        maze = MapGenerator.InitMap(mazeConfig);
        surface.BuildNavMesh();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetBackPos();
        }
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

    public Vector3 GetBackPos()
    {
        int wallSize = mazeConfig.wallSize;
        
        Vector3 playerPos = Player.Position;
        int posX = Convert.ToInt32(Math.Round(playerPos.x / 3));
        int posY = Convert.ToInt32(Math.Round(playerPos.z / 3));

        Vector3 lookDir = Player.Transform.forward;
        double lookX = lookDir.x;
        double lookY = lookDir.z;
        
        Tuple<int, int> firstDir;
        Tuple<int, int> secondDir;
        Tuple<int, int> thirdDir;
        
        // Quadrant 1
        if (lookX >= 0 && lookY >= 0)
        {
            if (lookX >= lookY)
            {
                firstDir = new Tuple<int, int>(-1, 0);
                secondDir = new Tuple<int, int>(0, -1);
                thirdDir = new Tuple<int, int>(0, 1);
            }
            else
            {
                firstDir = new Tuple<int, int>(0, -1);
                secondDir = new Tuple<int, int>(-1, 0);
                thirdDir = new Tuple<int, int>(1, 0);
            }
        }
        // Quadrant 2
        else if (lookX < 0 && lookY >= 0)
        {
            if (Math.Abs(lookX) <= Math.Abs(lookY))
            {
                firstDir = new Tuple<int, int>(0, -1);
                secondDir = new Tuple<int, int>(1, 0);
                thirdDir = new Tuple<int, int>(-1, 0);
            }
            else
            {
                firstDir = new Tuple<int, int>(1, 0);
                secondDir = new Tuple<int, int>(0, -1);
                thirdDir = new Tuple<int, int>(0, 1);
            }
        }
        // Quadrant 3
        else if (lookX < 0 && lookY < 0)
        {
            if (Math.Abs(lookX) >= Math.Abs(lookY))
            {
                firstDir = new Tuple<int, int>(1, 0);
                secondDir = new Tuple<int, int>(0, 1);
                thirdDir = new Tuple<int, int>(0, -1);
            }
            else
            {
                firstDir = new Tuple<int, int>(0, 1);
                secondDir = new Tuple<int, int>(1, 0);
                thirdDir = new Tuple<int, int>(-1, 0);
            }
        }
        // Quadrant 4
        else
        {
            if (Math.Abs(lookX) >= Math.Abs(lookY))
            {
                firstDir = new Tuple<int, int>(-1, 0);
                secondDir = new Tuple<int, int>(0, 1);
                thirdDir = new Tuple<int, int>(0, -1);
            }
            else
            {
                firstDir = new Tuple<int, int>(0, 1);
                secondDir = new Tuple<int, int>(-1, 0);
                thirdDir = new Tuple<int, int>(1, 0);
            }
        }

        int x = posX + firstDir.Item1;
        int y = posY + firstDir.Item2;
        if (maze[x, y] == 0)
        {
            return new Vector3(x * wallSize, 0, y * wallSize);
        }
        
        x = posX + secondDir.Item1;
        y = posY + secondDir.Item2;
        if (maze[x, y] == 0)
        {
            return new Vector3(x * wallSize, 0, y * wallSize);
        }
        
        x = posX + thirdDir.Item1;
        y = posY + thirdDir.Item2;
        if (maze[x, y] == 0)
        {
            return new Vector3(x * wallSize, 0, y * wallSize);
        }

        return new Vector3(-1, -1, -1);
    }
}
