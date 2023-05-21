using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ghost : MonoBehaviour
{
    private enum State
    {
        Dormant,    // * Check whether flashlight is on 
        Spawn,      // ^ Show particles and stuff
        Chase,       // & Chase player. Ghost is vulnerable to flashlight beams in this state
        Attack,     // ! Lunge at player
    }

    [SerializeField] private ParticleSystem _spawnEffect;
    [SerializeField] private ParticleSystem _passiveEffect;

    [SerializeField] private float _initialSpawnTimeThreshold = 25f; // how many seconds of flashlight off it takes for ghost to spawn
    [SerializeField] private float _spawnTimerChargeRate = 1f;
    [SerializeField] private float _spawnTimerDegradeRate = .5f;

    [SerializeField] private float _initialSpeed = 3f;

    [SerializeField] private int _initialAttackDamage = 10;
    [SerializeField] private float _attackRange = 4f;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private float _attackChargeUpDuration = .75f; // charge attack
    [SerializeField] private float _attackCooldownDuration = 1f; // if attack misses, cooldown

    // "level up" every successful attack
    [SerializeField] private int _maxLevel = 5;
    [SerializeField] private float _spawnTimeThresholdIncrement = 5f;
    [SerializeField] private float _speedIncrement = 1f;
    [SerializeField] private int _attackDamageIncrement = 10;

    private float _spawnTimeThreshold;
    private float _speed;
    private int _attackDamage;
    private int _level = 1;

    private State _state;
    private float _spawnTimer;

    private Material _material;
    private Collider _hitDetectCollider; // layer: playertrigger. enabled before attacking to check if attack hit
    private Coroutine _attackRoutine;

    [Header("TEMP SHIT")]
    [SerializeField] private Slider _spawnSlider;

    private void Start()
    {
        _hitDetectCollider = GetComponent<Collider>();
        _hitDetectCollider.enabled = false;
        _material = GetComponentInChildren<MeshRenderer>().material;

        _spawnTimeThreshold = _initialSpawnTimeThreshold;
        _spawnSlider.maxValue = _initialSpawnTimeThreshold;

        _speed = _initialSpeed;
        _attackDamage = _initialAttackDamage;
        
        GoToSleep();
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
            _spawnTimer += _spawnTimerChargeRate * Time.deltaTime;

            if (_spawnTimer >= _spawnTimeThreshold)
            {
                print("Ya fucked up. Ghost spawning");
                Spawn();
            }
        }
        else
        {
            _spawnTimer -= _spawnTimerDegradeRate * Time.deltaTime;

            if (_spawnTimer < 0f) _spawnTimer = 0f;
        }

        _spawnSlider.value = _spawnTimer;
    }

    private void UpdateChase()
    {
        transform.LookAt(transform.PlayerPositionHeightCorrected());

        if (transform.DistanceToPlayerHeightCorrected() <= _attackRange)
        {
            _attackRoutine = StartCoroutine(AttackRoutine());
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        }
    }

    private void Spawn()
    {
        _state = State.Spawn;
        _spawnEffect.Play();
        _passiveEffect.Play();

        // TODO: Spawn where player is looking at
        transform.position = Player.Position;

        StartCoroutine(SpawnRoutine());
    }

    private void GoToSleep()
    {
        _state = State.Dormant;
        _spawnTimer = 0f;
        _passiveEffect.Stop();
        SetAlpha(0f);
    }

    private IEnumerator SpawnRoutine()
    {
        float duration = _spawnEffect.main.duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(elapsed / duration);
            yield return null;
        }

        SetAlpha(1f);
        _state = State.Chase;
    }

    private IEnumerator AttackRoutine()
    {
        _state = State.Attack;

        Vector3 lungePos = transform.position + transform.DirectionToPlayerHeightCorrected() * 2f;

        yield return StartCoroutine(transform.SmoothMove(_attackChargeUpDuration, transform.position - transform.forward * 0.5f));

        _hitDetectCollider.enabled = true;
        yield return StartCoroutine(transform.SmoothMove(_attackDuration, lungePos));
        _hitDetectCollider.enabled = false;

        print("Attack miss!");

        // attack missed.
        yield return new WaitForSeconds(_attackCooldownDuration);

        _state = State.Chase;
    }

    private void OnTriggerEnter(Collider collider) // sucessful hit.
    {
        print("ghost hit!");

        _hitDetectCollider.enabled = false;
        StopCoroutine(_attackRoutine);

        Player.Health.TakeDamage(_attackDamage);

        if (_level != _maxLevel)
        {
            _level++;
            _spawnTimeThreshold += _spawnTimeThresholdIncrement;
            _spawnSlider.maxValue = _spawnTimeThreshold;

            _attackDamage += _attackDamageIncrement;
            _speed += _speedIncrement;
        }

        GoToSleep();
    }

    private void SetAlpha(float alpha)
    {
        Color color = _material.color;
        color.a = alpha;
        _material.color = color;
    }
}
