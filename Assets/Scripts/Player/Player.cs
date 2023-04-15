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
    }

    #endregion
    #region Public Properties

    public static PlayerMovement Movement { get { return Instance._move; } }
    public static PlayerLook Look { get { return Instance._look; } }
    public static PlayerHealth Health { get { return Instance._health; } }
    public static Flashlight Flashlight { get { return Instance._flash; } }

    public static Transform Transform { get { return Instance.transform; } }
    public static Vector3 Position { get { return Transform.position; } set { Transform.position = value; } }

    #endregion

    private PlayerMovement _move;
    private PlayerLook _look;
    private PlayerHealth _health; 

    [SerializeField] private Flashlight _flash;
    [SerializeField] private Transform _itemHolder;

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
            if (_itemEquipped != null)
                _itemEquipped.Use();
        }
    }

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
