using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestPlayer : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _sensitivity = 1f;

    private CharacterController _controller;
    private Transform _camTr;
    private float _camPitch;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camTr = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveVec = (transform.forward * moveZ + transform.right * moveX).normalized * _speed;
        _controller.SimpleMove(moveVec);

        float mouseX = Input.GetAxis("Mouse X") * _sensitivity; // horizontal look, rotate entire body along the y axis
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity; // vertical look, rotate only the cameraHolder along the x axis

        transform.Rotate(Vector3.up * mouseX);

        _camPitch = Mathf.Clamp(_camPitch - mouseY, -90f, 90f);
        _camTr.localEulerAngles = Vector3.right * _camPitch; 
    }
}
