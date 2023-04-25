using UnityEngine;
using UnityEngine.AI;

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
            int x = Random.Range(1, mapLength - 1);
            int y = Random.Range(1, mapLength - 1);
            if (maze[x, y] == 0) return new Vector3(x * wallSize, 0, y * wallSize);
        }
    }
}