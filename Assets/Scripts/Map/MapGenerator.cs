using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class MapGenerator
{
    [Header("MapConfig")]
    private static int mapSize;
    private static int wallSize;
    private static int wallHeight;
    private static int baseSize = 3;
    private static int holeNum = 240;
    private static int batteryNum = 10;
    private static int itemNum = 20;
    
    [Header("Maze")]
    private static int[,] maze;
    ////// Index //////
    // 0 for path    //
    // 1 for wall    //
    // 2 for exit    //
    // 3 for monster //
    // 4 for battery //
    // 5 for item    //
    ///////////////////
    
    [Header("Others")]
    private static Room[,] rooms;
    private static List<Tuple<int, int>> visited = new List<Tuple<int, int>>();
    private static Transform wallsHolder;
    private static Transform batteryHolder;
    private static Transform chestHolder;
    private static Random random = new Random();

    class Room
    {
        public Queue<Tuple<int, int>> RandomizedDir = new Queue<Tuple<int, int>>();
        private Tuple<int, int> _myDir;
        private List<Tuple<int, int>> _dir = new List<Tuple<int, int>>();
        
        public Room(int x, int y)
        {
            _myDir = new Tuple<int, int>(x, y);
            
            _dir.Add(new Tuple<int, int>(x-1, y));
            _dir.Add(new Tuple<int, int>(x+1, y));
            _dir.Add(new Tuple<int, int>(x, y-1));
            _dir.Add(new Tuple<int, int>(x, y+1));

            for (int idx = 4; idx > 0; idx--)
            {
                Tuple<int, int> chooseDir = _dir[random.Next(0, idx)];
                _dir.Remove(chooseDir);
                RandomizedDir.Enqueue(chooseDir);
            }
        }

        public Tuple<int, int> GetCurrentPos()
        {
            return _myDir;
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
    
    static int[,] DrawMap()
    {
        #region InitializeMaze
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                rooms[x, y] = new Room(x, y);
            }
        }
        int mazeLength = mapSize * 2 + 1;
        for (int x = 0; x < mazeLength; x++)
        {
            for (int y = 0; y < mazeLength; y++)
            {
                maze[x, y] = 1;
            }
        }
        #endregion
        
        MakeRoom(rooms[0, 0]);

        #region Draw Shortcuts
        int mapLength = mazeLength - 1;
        while (true)
        {
            if (holeNum == 0)
            {
                break;
            }

            int x = random.Next(1, mapLength);
            int y = random.Next(1, mapLength);
            if ((maze[y - 1, x] + maze[y + 1, x] == 2 && maze[y, x - 1] + maze[y, x + 1] == 0) || (maze[y - 1, x] + maze[y + 1, x] == 0 && maze[y, x - 1] + maze[y, x + 1] == 2))
            {
                maze[y, x] = 0;
                holeNum--;
            }
        }
        #endregion

        #region DrawExit
        List<Tuple<int, int>> randomExitLocations = new List<Tuple<int, int>>();
        randomExitLocations.Add(new Tuple<int, int>(mapSize, 0));
        randomExitLocations.Add(new Tuple<int, int>(0, mapSize));
        randomExitLocations.Add(new Tuple<int, int>(mapSize * 2, mapSize));
        randomExitLocations.Add(new Tuple<int, int>(mapSize, mapSize * 2));
        Tuple<int, int> exitLocation = randomExitLocations[random.Next(4)];
        int exitX = exitLocation.Item1;
        int exitY = exitLocation.Item2;
        maze[exitY, exitX] = 2;
        if (exitX == mapSize && exitY == 0)
        {
            maze[++exitY, exitX] = 0;
        }
        if (exitX == 0 && exitY == mapSize)
        {
            maze[exitY, ++exitX] = 0;
        }
        if (exitX == mapSize && exitY == mapSize * 2)
        {
            maze[--exitY, exitX] = 0;
        }
        if (exitX == mapSize * 2 && exitY == mapSize)
        {
            maze[exitY, --exitX] = 0;
        }
        #endregion

        #region DrawItemChests
        int itemSpawnCount = 0;
        while (itemSpawnCount < itemNum)
        {
            int x = random.Next(1, mazeLength - 1);
            int y = random.Next(1, mazeLength - 1);
            if (maze[y, x - 1] + maze[y, x + 1] + maze[y - 1, x] + maze[y + 1, x] == 3 && maze[y, x - 1] != 2 && maze[y, x + 1] != 2 && maze[y - 1, x] != 2 && maze[y + 1, x] != 2)
            {
                if (maze[y, x] == 0)
                {
                    maze[y, x] = 5;
                    itemSpawnCount++;
                }
            }
        }
        #endregion

        #region DrawBase
        for (int x = mapSize - baseSize; x <= mapSize + baseSize; x++)
        {
            for (int y = mapSize - baseSize; y <= mapSize + baseSize; y++)
            {
                maze[y, x] = 0;
            }
        }
        #endregion

        #region DrawBattery
        int[] allLocationY = {5, 9, 13, 17, 21, 39, 43, 47, 51, 55};
        for (int listIdx = 0; listIdx < 10; listIdx++)
        {
            int y = allLocationY[listIdx];
            int x = random.Next(mapSize) * 2 + 1;
            maze[y, x] = 4;
        }
        #endregion
        
        return maze;
    }

    static void MakeRoom(Room currentRoom)
    {
        Tuple<int, int> currentPos = currentRoom.GetCurrentPos();
        int currentX = currentPos.Item1;
        int currentY = currentPos.Item2;
        
        visited.Add(new Tuple<int, int>(currentX, currentY));
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
                    MakeRoom(rooms[nextX, nextY]);
                }
            }
        }
    }
    
    public static int[,] InitMap(MazeConfiguration config)
    {
        mapSize = config.mapSize;
        wallSize = config.wallSize;
        wallHeight = config.wallHeight;
        baseSize = config.baseSize;
        GameObject wallPrefab = config.wallPrefab;
        GameObject batteryPrefab = config.batteryPrefab;
        GameObject exitPrebaf = config.exitPrefab;
        GameObject chestPrefab = config.chestPrefab;
        GameObject[] monsterList = config.monsterPrefab;

        rooms = new Room[mapSize, mapSize];
        maze = new int[mapSize * 2 + 1, mapSize * 2 + 1];

        int[,] map = DrawMap();

        wallsHolder = new GameObject("Walls").transform;
        batteryHolder = new GameObject("Batteries").transform;
        chestHolder = new GameObject("Chests").transform;

        // Generate Wall and Battery
        int mazeLength = mapSize * 2 + 1;
        for (int x = 0; x < mazeLength; x++)
        {
            for (int y = 0; y < mazeLength; y++)
            {
                if (map[x, y] == 1)
                {
                    GameObject.Instantiate(wallPrefab, new Vector3(x*wallSize, wallHeight / 2, y*wallSize), Quaternion.identity, wallsHolder);
                }

                if (map[x, y] == 4)
                {
                    GameObject.Instantiate(batteryPrefab, new Vector3(x * wallSize, 1, y * wallSize),
                        Quaternion.identity, batteryHolder);
                }

                if (map[x, y] == 2)
                {
                    GameObject.Instantiate(exitPrebaf, new Vector3(x * wallSize, wallHeight / 2, y * wallSize), Quaternion.identity, wallsHolder);
                }

                if (map[x, y] == 5)
                {
                    GameObject.Instantiate(chestPrefab, new Vector3(x * wallSize, 1, y * wallSize),
                        Quaternion.identity, chestHolder);
                }
            }
        }

        #region SpawnMonsters
        // Configs
        List<GameObject> spawnedMonster = new List<GameObject>();
        int monsterNum = 3;
        int setSpawnLengthInterval = 3;
        int spawnLengthInterval = setSpawnLengthInterval * 2 + 1;

        // Randomizing
        List<Tuple<int, int>> spawnLocations = new List<Tuple<int, int>>();
        spawnLocations.Add(new Tuple<int, int>(spawnLengthInterval, spawnLengthInterval));
        spawnLocations.Add(new Tuple<int, int>(spawnLengthInterval, mazeLength - 1 - spawnLengthInterval));
        spawnLocations.Add(new Tuple<int, int>(mazeLength - 1 - spawnLengthInterval, spawnLengthInterval));
        spawnLocations.Add(new Tuple<int, int>(mazeLength - 1 - spawnLengthInterval, mazeLength - 1 - spawnLengthInterval));
        Queue<Tuple<int, int>> randomizedLocation = new Queue<Tuple<int, int>>();
        for (int idx = 4; idx > 0; idx--)
        {
            Tuple<int, int> chooseLocation = spawnLocations[random.Next(0, idx)];
            spawnLocations.Remove(chooseLocation);
            randomizedLocation.Enqueue(chooseLocation);
        }
        // Spawning
        for (int i = 0; i < monsterNum; i++)
        {
            GameObject monster = monsterList[random.Next(monsterList.Length)];
            if (spawnedMonster.Contains(monster))
            {
                i--;
                continue;
            }
            spawnedMonster.Add(monster);
            Tuple<int, int> spawnLocation = randomizedLocation.Dequeue();
            maze[spawnLocation.Item2, spawnLocation.Item1] = 3;
            int spawnX = spawnLocation.Item1 * wallSize;
            int spawnY = spawnLocation.Item2 * wallSize;
            GameObject.Instantiate(monster, new Vector3(spawnX, 1, spawnY), Quaternion.identity);
        }
        #endregion
        
        return map;
    }

}

[System.Serializable]
public class MazeConfiguration
{
    public int mapSize;
    public int wallSize;
    public int wallHeight;
    public int baseSize;
    public GameObject wallPrefab;
    public GameObject exitPrefab;
    public GameObject batteryPrefab;
    public GameObject chestPrefab;
    public GameObject[] monsterPrefab;
}