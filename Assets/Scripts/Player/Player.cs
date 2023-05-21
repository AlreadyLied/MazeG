using UnityEngine;

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

    // use this method to initialize non-static fields
    private static void InitFields()
    {
        _instance._move = _instance.GetComponent<PlayerMovement>();
        _instance._look = _instance.GetComponent<PlayerLook>();
        _instance._health = _instance.GetComponent<PlayerHealth>();
        _instance._items = _instance.GetComponent<PlayerItems>();
    }

    #endregion
    #region Public Properties

    public static PlayerMovement Movement { get { return Instance._move; } }
    public static PlayerLook Look { get { return Instance._look; } }
    public static PlayerHealth Health { get { return Instance._health; } }
    public static Flashlight Flashlight { get { return Instance._flash; } }
    public static PlayerItems Items { get { return Instance._items; } }

    public static Transform Transform { get { return Instance.transform; } }
    public static Vector3 Position { get { return Transform.position; } set { Transform.position = value; } }

    #endregion

    private PlayerMovement _move;
    private PlayerLook _look;
    private PlayerHealth _health; 
    private PlayerItems _items;

    [SerializeField] private Flashlight _flash;

    #region MonoBehaviour Methods
    
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            _look.GetFocusedInteractable()?.OnInteract();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            _health.TakeDamage(120);
        }
    }

    #endregion
    #region Public Methods

    // go limp, disable components and notify others
    public void OnDeath() 
    {
        _move.DropDead();

        _move.enabled = false;
        _look.enabled = false;
        _health.enabled = false;
        _items.enabled = false;
        enabled = false;

        UIManager.instance.PlayerDied();
    }

    #endregion

}
