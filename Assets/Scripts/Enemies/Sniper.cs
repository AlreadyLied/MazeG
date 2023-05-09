using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetect))]
public class Sniper : MonoBehaviour
{
    private enum State
    {
        Idle,
        Alert,
        Chase,
        Aim,
        Reload,
    }

    [SerializeField] private Transform _bulletExitTr;

    [SerializeField] private float _snipingDistance = 20f;
    [SerializeField] private float _snipeChargeDuration = 3f;
    [SerializeField] private int _snipeDamage = 75;
    [Range(0f, 1f)]
    [SerializeField] private float _aimSpeed = 0.1f;
    [SerializeField] private float _reloadDuration = 1f;

    [SerializeField] private float _alertDuration = 5f;

    private State _state = State.Idle;
    private NavMeshAgent _agent;
    private PlayerDetect _detect;
    private LineRenderer _aimLine;
    private int _aimLineCollideMask = (1 << (int)Layer.Player) | (1 << (int)Layer.Wall);
    private float _alertTimer;
    private float _snipeChargeTimer;

    #region Monobehaviour Methods

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _detect = GetComponent<PlayerDetect>();
        _detect.Register(OnPlayerDetect, OnPlayerLost);

        _aimLine = GetComponent<LineRenderer>();
        _aimLine.enabled = false;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Alert:
                UpdateAlert();
                break;

            case State.Chase:
                UpdateChase();
                break;
            
            case State.Aim:
                UpdateAim();
                break;
        }
    }

    #endregion
    #region FSM Update Functions

    private void UpdateIdle()
    {
        if (_agent.remainingDistance < 0.5f)
        {
            _agent.SetDestination(MapManager.instance.GetRandomPos());
        }
    }

    private void UpdateAlert()
    {
        _alertTimer -= Time.deltaTime;

        if (_alertTimer > 0f) // ^ chase Player even after losing sight
        {
            transform.LerpLookRotation(Player.Position, .5f);
            _agent.SetDestination(Player.Position);
        }
        else // * Alert timer run out: Transition to Idle state 
        {
            _state = State.Idle;
            _agent.updateRotation = true;
            _agent.stoppingDistance = 0f;
        }
    }

    private void UpdateChase()
    {
        if (_agent.remainingDistance <= _snipingDistance)
        {
            _state = State.Aim;
            _snipeChargeTimer = _snipeChargeDuration;
            _agent.SetDestination(transform.position);
            _aimLine.enabled = true;
        }
        else
        {
            _agent.SetDestination(Player.Position);
            transform.LerpLookRotation(Player.Position, 0.5f);
        }
    }

    private void UpdateAim()
    {
        Vector3 aimShake = Random.onUnitSphere;
        transform.LerpLookRotation(Player.Position + aimShake, _aimSpeed);

        _aimLine.SetPosition(0, _bulletExitTr.position);
        bool hitPlayer = false;

        if (Physics.Raycast(_bulletExitTr.position, _bulletExitTr.forward, out RaycastHit hit, Mathf.Infinity, _aimLineCollideMask))
        {
            _aimLine.SetPosition(1, hit.point + _bulletExitTr.forward * 0.25f);
            if (hit.transform.gameObject.layer == (int)Layer.Player)
                hitPlayer = true;
        }

        _snipeChargeTimer -= Time.deltaTime;

        if (_snipeChargeTimer <= 0f)
        {
            // shoot
            Shoot(hitPlayer);
        }
    }

    #endregion
    #region Player Detection

    private void OnPlayerDetect()
    {
        _state = State.Chase;
    }

    private void OnPlayerLost()
    {
        _state = State.Alert;
        _agent.stoppingDistance = 0f;
        _alertTimer = _alertDuration;
        _aimLine.enabled = false;
    }

    #endregion

    private void Shoot(bool hitPlayer)
    {
        print("Bang");

        if (hitPlayer)
        {
            Player.Health.TakeDamage(_snipeDamage);
        }

        _aimLine.enabled = false;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        _state = State.Reload;

        yield return new WaitForSeconds(_reloadDuration);

        if (_detect.detected) _state = State.Chase;
        else _state = State.Alert;
    }
}
