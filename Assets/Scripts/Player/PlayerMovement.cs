using UnityEngine;
using System.Collections;
using UnityEngine.UI; // TEMP

public class PlayerMovement : MonoBehaviour
{
    #region Fields

    [Header("General")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _jumpForce = 200f;

    [Header("Stamina")]
    [SerializeField] private float _staminaDrainRate = 1/8f;
    [SerializeField] private float _staminaRechargeRate = 1/2f;
    [SerializeField] private float _staminaRechargeDelay = 1.5f; // How many seconds of not running it takes for stamina to recharge
    [SerializeField] private float _jumpStaminaCost = 5f;

    [Header("TEMP SHIT")]
    public Text staminaText;

    void UpdateText() 
    {
        staminaText.text = $"Stamina: {_stamina:.00}";
    }
    public Rigidbody body { get; private set; }
    private Vector3 _moveVec;
    private float _stamina = 100f;
    private float _nextStaminaRecharge; // Have to surpass this time for stamina to recharge
    private bool _canMove = true;
    private Coroutine _bindRoutine; // to prevent overlapping binds


    #endregion
    #region Monobehaviour Methods

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_canMove)
        {
            _moveVec = Vector3.zero;

            if (_stamina < 100f && Time.time >= _nextStaminaRecharge)
            {
                _stamina += _staminaRechargeRate * Time.deltaTime;
            }

            _stamina = Mathf.Clamp(_stamina, 0f, 100f);

            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float speed;

        // If shift held down and non zero movement 
        if (Input.GetKey(KeyCode.LeftShift) && (moveX != 0f || moveZ != 0f))
        {
            if (_stamina > 0f)
            {
                speed = _runSpeed;
                _stamina -= _staminaDrainRate * Time.deltaTime;
            }
            else
            {
                speed = _walkSpeed;
            }

            // Have to completely let go of shift (if moving) for stamina recharge
            _nextStaminaRecharge = Time.time + _staminaRechargeDelay;
        }
        else
        {
            if (_stamina < 100f && Time.time >= _nextStaminaRecharge)
            {
                _stamina += _staminaRechargeRate * Time.deltaTime;
            }

            speed = _walkSpeed;
        }

        _stamina = Mathf.Clamp(_stamina, 0f, 100f);


        _moveVec = (transform.forward * moveZ + transform.right * moveX).normalized * speed;

        // TODO: Ground checking
        if (Input.GetKeyDown(KeyCode.Space) && _stamina >= _jumpStaminaCost)
        {
            body.AddForce(Vector3.up * _jumpForce);
            _stamina -= _jumpStaminaCost;
            _nextStaminaRecharge = Time.time + _staminaRechargeDelay;
        }


        UpdateText();
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + _moveVec * Time.fixedDeltaTime);
    }


    #endregion
    #region My Methods


    // constrainPos:    Where the player should be bound to
    // smoothMoveTime:  How many seconds to move to constrainPos
    public void Bind(float duration, Vector3? constrainPos = null, float smoothMoveTime = 0f)
    {
        if (_bindRoutine != null) StopCoroutine(_bindRoutine);
        _bindRoutine = StartCoroutine(BindRoutine(duration, constrainPos, smoothMoveTime));
    }

    private IEnumerator BindRoutine(float duration, Vector3? constrainPos = null, float smoothMoveTime = 0f)
    {
        _canMove = false;

        if (constrainPos != null)
        {
            smoothMoveTime = Mathf.Min(duration, smoothMoveTime);
            yield return StartCoroutine(transform.SmoothMove(smoothMoveTime, (Vector3)constrainPos));

            duration -= smoothMoveTime;
        }
        
        yield return new WaitForSeconds(duration);

        _canMove = true;
    }


    #endregion
}
