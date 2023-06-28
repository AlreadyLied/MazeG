using UnityEngine;

public class TestWeepingAngel : MonoBehaviour
{
    [SerializeField] private Transform _playerTr;
    [SerializeField] private int _checkFrameInterval = 100;

    private Camera _cam;
    private Collider _collider;
    private int _obstructionMask = 1 << (int)Layer.Wall;
    private Vector3[] _boundingPoints;

    private bool _canTeleport;

    private void Start()
    {
        _cam = Camera.main;
        _collider = GetComponent<Collider>();
        _boundingPoints = new Vector3[8];
        UpdateBoundingPoints();
    }

    /*
    * if not in sight, teleport behind player
    * don't teleport again until player acknowledges 
    * can only teleport if entered sight of player at least once after last teleporting
    */

    private void Update()
    {
        if (Time.frameCount % _checkFrameInterval == 0)
        {
            if (InCameraView())
            {
                _canTeleport = true;
            }
            else
            {
                if (_canTeleport)
                {
                    Teleport();
                }
            }
        }
    }

    private void Teleport()
    {
        Vector3 dest = _playerTr.position - _playerTr.forward * 3f;
        dest.y = 0;

        transform.position = dest;
        // UpdateBoundingPoints();

        _canTeleport = false;

        print("teleport");
    }

    private void UpdateBoundingPoints()
    {
        Bounds bounds = _collider.bounds;
        _boundingPoints[0] = bounds.min;
        _boundingPoints[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        _boundingPoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        _boundingPoints[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        _boundingPoints[4] = bounds.max;
        _boundingPoints[5] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        _boundingPoints[6] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        _boundingPoints[7] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }

    private bool InCameraView()
    {
        Vector3 camPos = _cam.transform.position;

        UpdateBoundingPoints();

        // check frustum
        // check raycast
        foreach (Vector3 vertex in _boundingPoints)
        {
            Vector3 viewportPoint = _cam.WorldToViewportPoint(vertex);

            // check if within viewport
            if (viewportPoint.z < 0f || viewportPoint.x < 0f || viewportPoint.x > 1f || viewportPoint.y < 0f || viewportPoint.y > 1f)
            {
                continue;
            }

            // check obstruction
            if (!Physics.Linecast(camPos, vertex, _obstructionMask))
            {
                return true;
            }
        }

        return false;
    }
}
