using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerLook))]
public class Player : MonoBehaviour
{
    #region Singleton Stuff

    private static Player _instance;
    public static Player Instance 
    {
        get
        {
            if (_instance == null)
            {
                // This will only happen if another script tries to access
                // the instance before this script's awake is called
                _instance = FindObjectOfType<Player>();

                if (_instance == null)
                {
                    Debug.LogError("Player instance missing!");
                    return null;
                }

                InitFields();
            }

            return _instance;
        }
    }

    private PlayerMovement _move;
    private PlayerLook _look;

    public static PlayerMovement Movement { get { return Instance._move; } }
    public static PlayerLook Look { get { return Instance._look; } }

    public static Transform Transform { get { return Instance.transform; } }
    public static Vector3 Position { get { return Transform.position; } set { Transform.position = value; } }

    // use this method to initialize non-static fields
    private static void InitFields()
    {
        _instance._move = _instance.GetComponent<PlayerMovement>();
        _instance._look = _instance.GetComponent<PlayerLook>();
    }

    #endregion

    [SerializeField] private Flashlight _flash;
    [SerializeField] private Transform _itemHolder;
    [SerializeField] private Text _healthText; // TEMP

    private int _health = 100;
    void SetHealthText() => _healthText.text = $"Health: {_health}";

    [SerializeField] private int _bleedIntervalSeconds = 1;
    private int _bleedPerTick;

    private Item _itemEquipped;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InitFields();
        }
        else if (_instance != this)
        {
            Debug.LogError("Multiple player instances detected.");
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    // Hanlde inputs regarding items and stuff
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _flash.Toggle();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _flash.ChargeBattery(20f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _look.GetFocusedInteractable()?.OnInteract();
        }
        if (Input.GetMouseButton(0))
        {
            _itemEquipped?.Use();
        }
    }

    #region Health Stuff

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            // FUCKING DIE
        }
        SetHealthText();
    }

    public void Bleed(int tickDamage, int tickCount)
    {
        StartCoroutine(BleedRoutineHelper(tickDamage, tickCount));

        if (_bleedPerTick == tickDamage)
        {
            StartCoroutine(BleedRoutine());
        }
    }

    private IEnumerator BleedRoutineHelper(int tickDamage, int tickCount)
    {
        _bleedPerTick += tickDamage;

        yield return new WaitForSeconds(tickCount * _bleedIntervalSeconds);

        _bleedPerTick -= tickDamage;
    }

    private IEnumerator BleedRoutine()
    {
        while (_bleedPerTick != 0)
        {
            TakeDamage(_bleedPerTick);

            yield return new WaitForSeconds(_bleedIntervalSeconds);
        }
    }
    
    public void Heal(int health)
    {
        _health = Mathf.Min(_health + health, 100);
        SetHealthText();
    }

    #endregion
    #region Item Stuff

    public void EquipItem(Item item)
    {
        // TODO
        if (_itemEquipped != null) return;

        item.transform.SetParent(_itemHolder);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        _itemEquipped = item;
    }

    #endregion
}
