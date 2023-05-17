using System;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator
{
    private static Room[,] rooms;
    private static List<Tuple<int, int>> visited;
    private static int[,] maze;
    ////// Index //////
    // path        0 //
    // wall        1 //
    // exit       10 //
    // monster    20 //
    // item       30 //
    // battery    40 //
    // base       50 //
    ///////////////////

    private static Transform wallHolder;
    private static Transform chestHolder;
    private static Transform batteryHolder;

    public static Vector3 exitPosition;

    class Room
    {
        public Tuple<int, int> MyPos;
        public Queue<Tuple<int, int>> RandomizedDir = new();
        public List<Tuple<int, int>> FourDir = new();

        public Room(int x, int y)
        {
             MyPos = new Tuple<int, int>(x, y);
             
             FourDir.Add(new Tuple<int, int>(x-1, y));
             FourDir.Add(new Tuple<int, int>(x+1, y));
             FourDir.Add(new Tuple<int, int>(x, y-1));
             FourDir.Add(new Tuple<int, int>(x, y+1));

             for (int idx = 4; idx > 0; idx--)
             {
                 Tuple<int, int> chooseDir = FourDir[Random.Range(0, idx)];
                 FourDir.Remove(chooseDir);
                 RandomizedDir.Enqueue(chooseDir);
             }
        }

        public Tuple<int, int> GetCurrentPos()
        {
            return MyPos;
        }

        public Tuple<int, int> GetNextPos()
        {
            if (RandomizedDir.Count == 0)
            {
                return null;
            }
            return RandomizedDir.Dequeue();
        }
    }

    private static void DrawMap(MazeConfiguration config)
    {
        int mapSize = config.mapSize;
        int holeNum = config.holeNum;
        int baseSize = config.baseSize;
        int mapLength = mapSize * 2 + 1;

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                rooms[x, y] = new Room(x, y);
            }
        }
        for (int x = 0; x < mapLength; x++)
        {
            for (int y = 0; y < mapLength; y++)
            {
                maze[x, y] = 1;
            }
        }

        visited = new List<Tuple<int, int>>();
        MakeRoom(rooms[0, 0], mapSize);

        int mapEdge = mapLength - 1;
        while (true)
        {
            if (holeNum == 0) break;

            int x = Random.Range(1, mapEdge);
            int y = Random.Range(1, mapEdge);
            if ((maze[y - 1, x] + maze[y + 1, x] == 2 && maze[y, x - 1] + maze[y, x + 1] == 0) || (maze[y - 1, x] + maze[y + 1, x] == 0 && maze[y, x - 1] + maze[y, x + 1] == 2))
            {
                maze[y, x] = 0;
                holeNum--;
            }
        }

        List<Tuple<int, int>> randomExitLocations = new List<Tuple<int, int>>();
        randomExitLocations.Add(new Tuple<int, int>(mapSize, 0));
        randomExitLocations.Add(new Tuple<int, int>(0, mapSize));
        randomExitLocations.Add(new Tuple<int, int>(mapSize * 2, mapSize));
        randomExitLocations.Add(new Tuple<int, int>(mapSize, mapSize * 2));

        Tuple<int, int> exitLocation = randomExitLocations[Random.Range(0, randomExitLocations.Count)];
        int exitX = exitLocation.Item1;
        int exitY = exitLocation.Item2;
        maze[exitY, exitX] = 10;
        if (exitX == mapSize && exitY == 0) maze[++exitY, exitX] = 0;
        if (exitX == 0 && exitY == mapSize) maze[exitY, ++exitX] = 0;
        if (exitX == mapSize && exitY == mapSize * 2) maze[--exitY, exitX] = 0;
        if (exitX == mapSize * 2 && exitY == mapSize) maze[exitY, --exitX] = 0;

        int smallestBasePos = mapSize - baseSize;
        int biggestBasePos = mapSize + baseSize;
        for (int x = smallestBasePos; x <= biggestBasePos; x++)
        {
            for (int y = smallestBasePos; y <= biggestBasePos; y++)
            {
                maze[y, x] = 50;
            }
        }
    }

    private static void MakeRoom(Room currentRoom, int mapSize)
    {
        Tuple<int, int> currentPos = currentRoom.GetCurrentPos();
        visited.Add(currentPos);

        int currentX = currentPos.Item1;
        int currentY = currentPos.Item2;
        maze[currentX * 2 + 1, currentY * 2 + 1] = 0;

        while (currentRoom.RandomizedDir.Count != 0)
        {
            Tuple<int, int> nextDir = currentRoom.GetNextPos();
            int nextX = nextDir.Item1;
            int nextY = nextDir.Item2;

            if (0 <= nextX && nextX < mapSize && 0 <= nextY && nextY < mapSize)
            {
                if (!visited.Contains(nextDir))
                {
                    maze[currentX + nextX + 1, currentY + nextY + 1] = 0;
                    MakeRoom(rooms[nextX, nextY], mapSize);
                }
            }
        }
    }

    public static int[,] InitMap(MazeConfiguration config)
    {
        int mapSize = config.mapSize;
        int mapLength = mapSize * 2 + 1;

        rooms = new Room[mapSize, mapSize];
        maze = new int[mapLength, mapLength];
        DrawMap(config);
        
        GenerateMap(config);
        SpawnItemChests(config);
        SpawnBatteries(config);

        return maze;
    }

    private static void GenerateMap(MazeConfiguration config)
    {
        int mapSize = config.mapSize;
        int mapLength = mapSize * 2 + 1;
        int wallSize = config.wallSize;
        int wallHeight = config.wallHeight;
        
        GameObject wallPrefab = config.wallPrefab;
        GameObject exitPrefab = config.exitPrefab;
        wallHolder = new GameObject("WallHolder").transform;
        
        for (int x = 0; x < mapLength; x++)
        {
            for (int y = 0; y < mapLength; y++)
            {
                if (maze[x, y] == 1)
                {
                    GameObject.Instantiate(wallPrefab, new Vector3(x * wallSize, wallHeight / 2, y * wallSize), Quaternion.identity, wallHolder);
                }

                if (maze[x, y] == 10)
                {
                    GameObject.Instantiate(exitPrefab, new Vector3(x * wallSize, wallHeight / 2, y * wallSize),
                        Quaternion.identity);
                    exitPosition = new Vector3(x * wallSize, wallHeight / 2, y * wallSize);
                }
            }
        }
    }

    private static void SpawnMonster(MazeConfiguration config)
    {
        // int mapSize = config.mapSize;
        // int mapLength = mapSize * 2 + 1;
        // int wallSize = config.wallSize;
        // int monsterNum = config.monsterNum;
        // int lengthFromEdge = 7;
        //
        // GameObject[] monsterPrefabs = config.monsterPrefabs;
        // List<GameObject> spawnedMonsters = new();
        //
        // List<Tuple<int, int>> spawnLocations = new();
        // Queue<Tuple<int, int>> randomLocation = new();
        // spawnLocations.Add(new Tuple<int, int>(lengthFromEdge, lengthFromEdge));
        // spawnLocations.Add(new Tuple<int, int>(lengthFromEdge, mapLength - lengthFromEdge - 1));
        // spawnLocations.Add(new Tuple<int, int>(mapLength - lengthFromEdge - 1, lengthFromEdge));
        // spawnLocations.Add(new Tuple<int, int>(mapLength - lengthFromEdge - 1, mapLength - lengthFromEdge - 1));
        // for (int idx = 4; idx > 0; idx--)
        // {
        //     Tuple<int, int> chooseLocation = spawnLocations[Random.Range(0, idx)];
        //     spawnLocations.Remove(chooseLocation);
        //     randomLocation.Enqueue(chooseLocation);
        // }
        //
        // for (int spawnCount = 1; spawnCount <= monsterNum; spawnCount++)
        // {
        //     GameObject monster = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
        //     if (spawnedMonsters.Contains(monster))
        //     {
        //         spawnCount--;
        //         continue;
        //     }
        //     spawnedMonsters.Add(monster);
        //     Tuple<int, int> spawnLocation = randomLocation.Dequeue();
        //     int spawnX = spawnLocation.Item1 * wallSize;
        //     int spawnY = spawnLocation.Item2 * wallSize;
        //     GameObject.Instantiate(monster, new Vector3(spawnX, 1, spawnY), Quaternion.identity);
        // }
    }

    private static void SpawnItemChests(MazeConfiguration config)
    {
        int mapSize = config.mapSize;
        int mapLength = mapSize * 2 + 1;
        int wallSize = config.wallSize;
        int chestNum = config.chestNum;
        
        GameObject chestPrefab = config.chestPrefab;
        chestHolder = new GameObject("ChestHolder").transform;
        
        int chestSpawnCount = 0;
        while (chestSpawnCount < chestNum)
        {
            int x = Random.Range(1, mapLength - 1);
            int y = Random.Range(1, mapLength - 1);
            if (maze[x - 1, y] + maze[x + 1, y] + maze[x, y - 1] + maze[x, y + 1] == 3 && maze[x, y] == 0)
            {
                GameObject.Instantiate(chestPrefab, new Vector3(x * wallSize, 1, y * wallSize), Quaternion.identity, chestHolder);
                maze[x, y] = 30;
                chestSpawnCount++;
            }
        }
    }

    private static void SpawnBatteries(MazeConfiguration config)
    {
        int mapSize = config.mapSize;
        int mapLength = mapSize * 2 + 1;
        int wallSize = config.wallSize;
        int batteryNum = config.batteryNum;
        
        GameObject batteryPrefab = config.batteryPrefab;
        batteryHolder = new GameObject("BatteryHolder").transform;

        int batterySpawnCount = 0;
        while (batterySpawnCount < batteryNum)
        {
            int x = Random.Range(1, mapLength - 1);
            int y = Random.Range(1, mapLength - 1);
            if (maze[x, y] == 0)
            {
                GameObject.Instantiate(batteryPrefab, new Vector3(x * wallSize, 1, y * wallSize), Quaternion.identity,
                    batteryHolder);
                maze[x, y] = 40;
                batterySpawnCount++;
            }
        }
    }
}

[Serializable]
public class MazeConfiguration
{
    [Header("Map")]
    public int mapSize;
    public int holeNum;
    public int baseSize;
    
    [Header("Wall")]
    public int wallSize;
    public int wallHeight;

    [Header("Nums")]
    public int chestNum;
    public int batteryNum;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject exitPrefab;
    public GameObject chestPrefab;
    public GameObject batteryPrefab;
    public GameObject[] monsterPrefabs;
    public GameObject zombiePrefab;
}