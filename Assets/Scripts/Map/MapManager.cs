using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    private static int[,] maze;
    [SerializeField] private MazeConfiguration mazeConfig;
    [SerializeField] private NavMeshSurface surface;

    private void Awake()
    {
        maze = MapGenerator.InitMap(mazeConfig);
        surface.BuildNavMesh();
    }

    private void Start()
    {
        Player.Position = (Vector3.forward + Vector3.right) * mazeConfig.mapSize * mazeConfig.wallSize;
    }
}