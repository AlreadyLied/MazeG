using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerDetect))]
[RequireComponent(typeof(NavMeshAgent))]
public class Hunter : MonoBehaviour
{
    private enum State 
    {
        Idle, // * Pick a random position, move there, and repeat
        Alert, // ^ Even after losing sight of player, chase him with omniscient knowledge about his position, for _alertDuration seconds
        Chase, // & Chase Player
        Attack, // ! Playing attack animation. No transitions occur in Attack state. 
    }

    private State _state = State.Idle;

    [Header("Attack Settings")]

    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackPushForce = 10f;
    [SerializeField] private float _attackDistance = 4f;
    [SerializeField] private float _attackChargeDuration = 3f;
    [SerializeField] private float _attackCooldown = 1f;

    [Space(10f)]

    [Header("Trap Settings")]

    [SerializeField] private BearTrap _bearTrapPrefab;
    [SerializeField] private float _bearTrapSetDistanceSqrd = 100f;
    [SerializeField] private int _maxBearTraps = 3;
    [SerializeField] private float _bearTrapSetAttemptInterval = 3f;

    [Space(10f)]
    
    [Header("Chase Settings")]
    
    [SerializeField] private float _alertDuration = 5f;

    private PlayerDetect _detect;
    private NavMeshAgent _agent;
    private float _alertTimer;
    private float _attackTimer;

    private BearTrap[] _bearTraps;
    private int _bearTrapsPlaced;
    private float _bearTrapAttemptTimer; // time til next trap set attempt

    #region Monobehaviour Methods

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(transform.position);

        _detect = GetComponent<PlayerDetect>();
        _detect.Register(OnPlayerDetect, OnPlayerLost);

        _bearTraps = new BearTrap[_maxBearTraps];
        for (int i = 0; i < _maxBearTraps; i++)
        {
            BearTrap bt = _bearTraps[i] = Instantiate(_bearTrapPrefab);
            bt.OnActivated += () => 
            {
                print("어떤 시바 새끼가 내 덫 밟았어!");
                _bearTrapsPlaced--;
            };
        }
        _bearTrapAttemptTimer = _bearTrapSetAttemptInterval;
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
            
            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    #endregion
    #region FSM Update Functions
    
    // * Pick a random position, move there, repeat
    // TODO: place beartraps
    private void UpdateIdle() 
    {
        if (_agent.remainingDistance < 0.5f)
        {
            _agent.SetDestination(MapManager.instance.GetRandomPos());
        }

        if (_bearTrapsPlaced != _maxBearTraps)
        {
            _bearTrapAttemptTimer -= Time.deltaTime;

            if (_bearTrapAttemptTimer <= 0f)
            {
                if (TrySetBearTrap())
                {
                    _bearTrapAttemptTimer = _bearTrapSetAttemptInterval;
                    _bearTrapsPlaced++;
                }
            }
        }
    }
    
    // ^ Even after losing sight of player, chase him with omniscient knowledge about his position, for _alertDuration seconds
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
            _bearTrapAttemptTimer = _bearTrapSetAttemptInterval;
        }
    }

    // & Chase Player
    private void UpdateChase()
    {
        // ! if Player in attack range, Transition to Attack state
        if (_agent.remainingDistance < _attackDistance)
        {
            _agent.SetDestination(transform.position); // ! No movement while attacking
            _attackTimer = _attackChargeDuration; // ! Start timer
            _state = State.Attack;
        }
        else
        {
            transform.LerpLookRotation(Player.Position, .5f);
            _agent.SetDestination(Player.Position);
        }
    }

    private void UpdateAttack()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer > 0f)
        {
            // ! Look at player during attack
            transform.LerpLookRotation(Player.Position, 0.75f);
        }
        else // & Attack, then transition to Chase state
        {
            Vector3 dirToPlayer = transform.DirectionToPlayerHeightCorrected();
            
            if (dirToPlayer.magnitude <= _attackDistance) // hit
            {
                Player.Health.TakeDamage(_attackDamage);
                Player.Movement.Push(dirToPlayer.normalized * _attackPushForce);

                _attackTimer = _attackCooldown; // ! On attack successful, don't chase Player for an additional _attackCooldown seconds 
            }
            else
            {
                if (_detect.detected) _state = State.Chase;
                else _state = State.Alert;
            }

        }
    }

    #endregion
    #region Player Detection

    private void OnPlayerDetect() // & Transition to Chase state.
    {
        if (_state != State.Attack) // ! Attack state ends only upon finishing current animation
        {
            _state = State.Chase; 
            _agent.stoppingDistance = _attackDistance; // & leave some distance for attack motions
            _agent.updateRotation = false; // & Rotation is updated via code with LookRotation in Chase state
        }
    }

    private void OnPlayerLost() // ^ Transition to Alert state
    {
        if (_state != State.Attack) // ! Attack state ends only upon finishing current animation
        {
            _state = State.Alert;
            _agent.stoppingDistance = 0f; // ^ No point stopping early if Player not in sight 
            _alertTimer = _alertDuration; // ^ Start timer
        }
    }

    #endregion

    private bool TrySetBearTrap()
    {
        BearTrap candidate = null;

        for (int i = 0; i < _maxBearTraps; i++)
        {
            BearTrap bt = _bearTraps[i];

            if (bt.trapActive)
            {
                if ((bt.transform.position - transform.position).sqrMagnitude <= _bearTrapSetDistanceSqrd) 
                    return false;
            }
            else candidate = bt;
        }

        candidate.Enable();
        candidate.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        return true;
    }

    private void OnDrawGizmos()
    {
        if (_agent == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_agent.destination, 0.5f);
    }
}
