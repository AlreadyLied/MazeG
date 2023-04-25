using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerDetect))]
public class Hunter : MonoBehaviour
{
    private enum State 
    {
        Idle, // * Pick a random position, move there, and repeat
        Alert, // ^ Even after losing sight of player, chase him with omniscient knowledge about his position, for _alertDuration seconds
        Chase, // ! Chase Player
    }

    private NavMeshAgent _agent;
    private Vector3 _destPos;
    private State _state = State.Idle;

    [SerializeField] private float _attackDistance;

    [SerializeField] private float _alertDuration;
    private float _alertTimer;

    private void Start()
    {
        GetComponent<PlayerDetect>().Register(OnPlayerDetect, OnPlayerLost);
        _agent = GetComponent<NavMeshAgent>();
        // _agent.updateRotation = false;
        _destPos = transform.position;
    }

    private void Update()
    {
        switch (_state)
        {
            // * Idle state: roam around randomly
            case State.Idle:

                // TODO: how's the overhead?
                if (_agent.remainingDistance < 0.5f)
                {
                    _destPos = MapManager.instance.GetRandomPos();
                    _agent.SetDestination(_destPos);
                }

                break;

            case State.Alert:
                
                _alertTimer -= Time.deltaTime;

                if (_alertTimer > 0f)
                {
                    _destPos = Player.Position; // ^ chase Player for _alertDuration seconds even after losing sight
                    transform.LookAt(_destPos);
                    _agent.SetDestination(_destPos);
                }
                else
                {
                    _agent.updateRotation = true;
                    _state = State.Idle; // ^ if Player not detected in _alertDuration seconds, transition to idle state
                }

                break;

            case State.Chase:

                _destPos = Player.Position;
                transform.LookAt(_destPos);
                _agent.SetDestination(_destPos);

                break;
        }

    }

    private void Attack()
    {
        
    }

    private void OnPlayerDetect()
    {
        print("FUUFUFUUFUFUFU");
        _state = State.Chase;
        _agent.stoppingDistance = _attackDistance; // leave some distance for attack motions
        _agent.updateRotation = false;
    }

    private void OnPlayerLost()
    {
        _state = State.Alert;
        _agent.stoppingDistance = 0f; 
        _alertTimer = _alertDuration; // start timer
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_destPos, 0.5f);
    }

}
