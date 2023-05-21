using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform _viewHolder;
    [SerializeField] private float _sensitivity = 1f;
    private float _camPitch;

    [SerializeField] private float _detectRange = 3f;
    private int _cameraRaycastLayer = (1 << (int)Layer.PlayerInteract);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity; // horizontal look, rotate entire body along the y axis
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity; // vertical look, rotate only the cameraHolder along the x axis

        transform.Rotate(Vector3.up * mouseX);

        _camPitch = Mathf.Clamp(_camPitch - mouseY, -90f, 90f);
        _viewHolder.localEulerAngles = Vector3.right * _camPitch; 
    }

    // returns the Interactable Object the player is looking at
    // returns null if none
    public Interactable GetFocusedInteractable()
    {
        if (Raycast(out RaycastHit hit, _cameraRaycastLayer, _detectRange))
            return hit.transform.GetComponent<Interactable>();
        return null;
    }

    public bool Raycast(out RaycastHit hit, int layerMask = ~0, float distance = Mathf.Infinity)
    {
        return Physics.Raycast(_viewHolder.position, _viewHolder.forward, out hit, distance, layerMask);
    }
}


