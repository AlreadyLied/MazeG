using UnityEngine;
using System.Collections;
using UnityEngine.UI; // TEMP

public class PlayerMovement : MonoBehaviour
{
    #region Fields

    [Header("General")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _runSpeed = 8f;

    [Header("Stamina")]
    [SerializeField] private float _staminaDrainRate = 1/8f;
    [SerializeField] private float _staminaRechargeRate = 1/2f;
    [SerializeField] private float _staminaRechargeDelay = 1.5f; // How many seconds of not running it takes for stamina to recharge

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpStaminaCost = 5f;
    [SerializeField] private float _gravityMultiplier = 1f; // cannot change mid run

    private CharacterController _controller;
    private Vector3 _yVelocity; // jump & gravity
    private Vector3 _gravity;
    private bool _canMove = true;
    private Coroutine _bindRoutine; // to prevent overlapping binds
    private float _stamina = 100f;
    private float _nextStaminaRecharge; // Have to surpass this time for stamina to recharge
    private float _speedMultiplier = 1f;
    private Coroutine _speedUpRoutine;
    private Coroutine _pushRoutine;

    #endregion
    #region Monobehaviour Methods

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _gravity = Physics.gravity * _gravityMultiplier;
    }

    private void Update()
    {
        // * Recharge stamina even while player is bound.
        if (!_canMove)
        {
            if (_stamina < 100f && Time.time >= _nextStaminaRecharge)
            {
                _stamina += _staminaRechargeRate * Time.deltaTime;
            }

            _stamina = Mathf.Clamp(_stamina, 0f, 100f);
            UIManager.instance.UpdateStamina(_stamina);
            
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float speed;

        // * if shift held down and non zero movement -> drain stamina
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

            // * Have to completely let go of shift (if moving) for stamina recharge
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

        Vector3 moveVec = (transform.forward * moveZ + transform.right * moveX)
                            .normalized * speed * _speedMultiplier;

        if (_controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space) && _stamina >= _jumpStaminaCost)
            {
                _yVelocity.y = _jumpForce;

                _stamina -= _jumpStaminaCost;
                _nextStaminaRecharge = Time.time + _staminaRechargeDelay;
            }
            else if (_yVelocity.y < 0f)
            {
                _yVelocity.y = 0f;
            }
        }

        _yVelocity += _gravity * Time.deltaTime; // sqrd
        _controller.Move((moveVec + _yVelocity) * Time.deltaTime);

        _stamina = Mathf.Clamp(_stamina, 0f, 100f);
        UIManager.instance.UpdateStamina(_stamina);
    }

    #endregion
    #region Public Methods

    // constrainPos:    Where the player should be bound to
    // smoothMoveTime:  How many seconds to move to constrainPos
    public void Bind(float duration, Vector3? constrainPos = null, float smoothMoveTime = 0f)
    {
        if (_bindRoutine != null) StopCoroutine(_bindRoutine);
        if (_pushRoutine != null) StopCoroutine(_pushRoutine); // if player gets bound while being pushed, the effect of pushing stops
        _bindRoutine = StartCoroutine(BindRoutine(duration, constrainPos, smoothMoveTime));
    }

    public void SpeedUp(float multiplier, int duration)
    {
        if (_speedUpRoutine != null) StopCoroutine(_speedUpRoutine);
        _speedUpRoutine = StartCoroutine(SpeedUpRoutine(multiplier, duration));
    }

    public void Push(Vector3 force)
    {
        if (_canMove) // * Pushing has no effect when player is bound
        {
            if (_pushRoutine != null) StopCoroutine(_pushRoutine); // No overlapping pushes
            _pushRoutine = StartCoroutine(PushRoutine(force));
        }
    }

    #endregion

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
    
    private IEnumerator SpeedUpRoutine(float multiplier, int duration)
    {
        _speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        _speedMultiplier = 1f;
    }

    private IEnumerator PushRoutine(Vector3 force)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            _controller.Move(force * (1.1f - (elapsed / duration)) * Time.deltaTime);

            yield return null;
        }
    }
}
