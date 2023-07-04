using UnityEngine;

public class MannequinLOSDetect : MonoBehaviour
{
    private BoxCollider _collider;
    private Vector3[] _detectPoints;
    private Camera _playerCam;
    private int _obstructionMask = 1 << (int)Layer.Wall;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _playerCam = Camera.main;

        _detectPoints = new Vector3[9];
        UpdatePoints();
    }

    public void UpdatePoints()
    {
        Bounds bounds = _collider.bounds;

        _detectPoints[0] = bounds.center;
        _detectPoints[1] = bounds.min;
        _detectPoints[2] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        _detectPoints[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        _detectPoints[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        _detectPoints[5] = bounds.max;
        _detectPoints[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        _detectPoints[7] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        _detectPoints[8] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }
    
    public bool InPlayerSight()
    {
        foreach (Vector3 point in _detectPoints)
        {
            Vector3 viewportPoint = _playerCam.WorldToViewportPoint(point);

            // check if within viewport
            if (viewportPoint.z < 0f || viewportPoint.x < 0f || viewportPoint.x > 1f || viewportPoint.y < 0f || viewportPoint.y > 1f)
            {
                continue;
            }

            // check obstruction
            if (!Physics.Linecast(_playerCam.transform.position, point, _obstructionMask))
            {
                Debug.DrawLine(_playerCam.transform.position, point, Color.green, .25f);
                return true;
            }
        }

        return false;
    }
}
