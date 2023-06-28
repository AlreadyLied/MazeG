using UnityEngine;

public class TestWeepingAngel : MonoBehaviour
{
    private Camera _cam;
    private Collider _collider;
    private Vector3[] _vertices;
    private int _obstructionMask = 1 << (int)Layer.Wall;

    private void Start()
    {
        _cam = Camera.main;
        _collider = GetComponent<Collider>();
        _vertices = new Vector3[8];

        UpdateVertices();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (InCameraView())
            {
                print("YES!");
            }
            else
            {
                print("NO!");
            }
        }
    }

    private void UpdateVertices()
    {
        Bounds bounds = _collider.bounds;
        _vertices[0] = bounds.min;
        _vertices[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        _vertices[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        _vertices[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        _vertices[4] = bounds.max;
        _vertices[5] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        _vertices[6] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        _vertices[7] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }

    private bool InCameraView()
    {
        // check frustum
        // check raycast
        foreach (Vector3 vertex in _vertices)
        {
            Vector3 viewportPoint = _cam.WorldToViewportPoint(vertex);

            // check if within viewport
            if (viewportPoint.z < 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
                continue;

            // check obstruction
            if (!Physics.Linecast(_cam.transform.position, vertex, _obstructionMask))
            {
                Debug.DrawLine(_cam.transform.position, vertex, Color.green, 0.1f);
                return true;
            }
        }

        return false;
    }
}
