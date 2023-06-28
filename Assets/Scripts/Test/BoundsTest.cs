using UnityEngine;

public class BoundsTest : MonoBehaviour
{
    private Transform[] _corners;
    private Matrix4x4 _thisMatrix;

    private Transform Shit(Vector3 vec)
    {
        Transform tr = new GameObject().transform;
        // tr.SetParent(transform);
        tr.position = _thisMatrix.MultiplyPoint3x4(vec);
        return tr;
    }

    private void Start()
    {
        Quaternion originalRotation = transform.rotation;

        _thisMatrix = transform.localToWorldMatrix;
        transform.rotation = Quaternion.identity;

        Vector3 extents = GetComponent<Collider>().bounds.extents;
        
        _corners = new Transform[8];
        _corners[0] = Shit(extents);
        _corners[1] = Shit(new Vector3(-extents.x, extents.y, extents.z));
        _corners[2] = Shit(new Vector3(extents.x, -extents.y, extents.z));
        _corners[3] = Shit(new Vector3(extents.x, extents.y, -extents.z));
        _corners[4] = Shit(-extents);
        _corners[5] = Shit(new Vector3(extents.x, -extents.y, -extents.z));
        _corners[6] = Shit(new Vector3(-extents.x, extents.y, -extents.z));
        _corners[7] = Shit(new Vector3(-extents.x, -extents.y, extents.z));

        transform.rotation = originalRotation;
    }

    private void OnDrawGizmos()
    {
        if (_corners == null)
            return;
        
        Gizmos.color = Color.red;
        foreach (Transform tr in _corners)
        {
            Gizmos.DrawSphere(tr.position, 0.1f);
        }
    }
}
