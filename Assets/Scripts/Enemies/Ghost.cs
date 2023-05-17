using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    private enum State
    {
        Dormant,    // * Periodically check whether flashlight is on 
        Spawn,      // ^ Show particles and stuff
        Chase,       // & Chase player. Ghost is vulnerable to flashlight beams in this state
        Attack,     // ! Lunge at player
    }

    [SerializeField] private float _flashOffDurationForSpawn = 25f; // how many seconds of flashlight off it takes for ghost to spawn
    [SerializeField] private ParticleSystem _spawnEffect;
    [SerializeField] private ParticleSystem _passiveEffect;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _attackDistance = 4f;
    [SerializeField] private float _attackDuration = 1f;

    private State _state = State.Dormant;
    private float _flashOffDuration;

    private Material _material;

    private void Start()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
        SetAlpha(0);
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Dormant:
                UpdateDormant();
                break;
            
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    private void UpdateDormant()
    {
        if (!Player.Flashlight.isOn)
        {
            _flashOffDuration += Time.deltaTime;

            if (_flashOffDuration >= _flashOffDurationForSpawn)
            {
                print("Ya fucked up. Ghost spawning");
                StartCoroutine(SpawnRoutine());
            }
        }
    }

    private void UpdateChase()
    {
        transform.LookAt(transform.PlayerPositionHeightCorrected());

        if (transform.DistanceToPlayerHeightCorrected() <= _attackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        _state = State.Spawn;
        _spawnEffect.Play();
        _passiveEffect.Play();

        float duration = _spawnEffect.main.duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(elapsed / duration);
            yield return null;
        }

        SetAlpha(1);
        _state = State.Chase;
    }

    private IEnumerator AttackRoutine()
    {
        _state = State.Attack;

        yield return StartCoroutine(transform.SmoothMove(_attackDuration, Player.Position));

        SetAlpha(0f);

        _flashOffDuration = 0f;
        _state = State.Dormant; // go to sleep
    }

    private void SetAlpha(float alpha)
    {
        Color color = _material.color;
        color.a = alpha;
        _material.color = color;
    }
}
