using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 4;
    [SerializeField] private Transform _itemHolder;

    private Item[] _inventory;
    private int _selectedIndex;
    private Item _selectedItem;

    private bool _flag;

    private void Start()
    {
        _inventory = new Item[_inventorySize];

        SelectItem(0); 
    }

    public void UseSelectedItem()
    {
        Item selected = _inventory[_selectedIndex];
        if (selected != null) selected.Use();
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= _inventorySize) return;

        // TODO: proper holster / draw animations
        if (_selectedItem != null) _selectedItem.gameObject.SetActive(false); // holster prev item

        _selectedIndex = index;
        _selectedItem = _inventory[_selectedIndex];
        if (_selectedItem != null)
        {
            _selectedItem.gameObject.SetActive(true);
            _selectedItem.transform.SetParent(_itemHolder);
            _selectedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void OnMouseScroll(int delta)
    {
        int newIndex = (_selectedIndex + delta) % _inventorySize;
        SelectItem(newIndex);
    }

    public bool AddItem(Item item)
    {
        for (int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)
            {
                _inventory[index] = item;
                item.itemIndex = index;
                UIManager.instance.AddToInventory(item);

                if (index == _selectedIndex)
                    SelectItem(index);
                else
                    item.gameObject.SetActive(false);

                return true;
            }
        }

        return false;
    }

}
