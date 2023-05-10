using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 4;
    [SerializeField] private Transform _itemHolder;

    private Item[] _inventory;
    private int _selectedIndex; // ! should only be modified via SelectItem

    private void Start()
    {
        _inventory = new Item[_inventorySize];

        SelectIndex(0); 
    }

    public void UseSelectedItem()
    {
        Item selected = _inventory[_selectedIndex];
        if (selected != null) selected.Use();
    }

    public void SelectIndex(int index) // ! _selectedIndex should only be modified via this method
    {
        // if index out of range or same as before, do nothing
        if (index < 0 || index >= _inventorySize || index == _selectedIndex) return;

        Item item = _inventory[_selectedIndex]; // prev item
        if (item != null) HolsterItem(item); // if exists, holster prev item

        item = _inventory[_selectedIndex = index]; // new item
        if (item != null) DrawItem(item);
    }

    public void OnMouseScroll(int delta)
    {
        int newIndex = (_selectedIndex + delta) % _inventorySize;
        if (newIndex < 0) newIndex += _inventorySize;
        SelectIndex(newIndex);
    }

    // * Returns true if item added successfully, else false (e.g. inventory is full)
    // TODO: Maybe a better approach would be to keep track of the number of unoccupied slots,
    // TODO: and have a public boolean property indicating whether the player's inventory is full or not
    // TODO: The main advantage of such approach would be that we could check whether AddItem is possible
    // TODO: prior to calling it, which allows more flexibility on how we deal with a full inventory.
    // TODO: One downside is that this script should be able to "know" when an item is used.
    // TODO: We could implement this quite easily using event callbacks, especially since the Item superclass
    // TODO: already has a concrete method "Used()", but it does create a dependency between classes. 
    public bool AddItem(Item item) 
    {
        for (int index = 0; index < _inventorySize; index++) // & Linear search _inventory for an empty slot
        {
            if (_inventory[index] == null)
            {
                _inventory[index] = item;
                item.itemIndex = index;
                UIManager.instance.AddToInventory(item); // & Update inventory UI

                item.transform.SetParent(_itemHolder);
                item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                if (index == _selectedIndex)    // * if item is added to current index
                    DrawItem(item);
                else
                    HolsterItem(item);

                return true;
            }
        }

        return false; // & Inventory full
    }

    private void HolsterItem(Item item)
    {
        item.gameObject.SetActive(false);
    }

    private void DrawItem(Item item)
    {
        item.gameObject.SetActive(true);
    }

}
