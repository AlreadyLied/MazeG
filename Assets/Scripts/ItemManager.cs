using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    
    public GameObject itemPopup;
    
    // Slot 1
    public Item healthPack;
    // Slot 2
    private List<Item> _slot2 = new List<Item>();
    public Item adrenalineShot;
    public Item invisibleCloak;
    public Item alarmDistraction;

    // Slot 3
    private List<Item> _slot3 = new List<Item>();
    public Item sprayCan;
    public Item pathfindingFairy;

    
    void Start()
    {
        instance = this;
        
        _slot2.Add(adrenalineShot);
        _slot2.Add(invisibleCloak);
        _slot2.Add(alarmDistraction);
        
        _slot3.Add(sprayCan);
        _slot3.Add(pathfindingFairy);
    }

    public void ShowItems()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        itemPopup.SetActive(true);
    }

    public void ItemSelectSlot1()
    {
        Player.Instance.EquipItem(Instantiate(healthPack));
        ClosePopup();
    }

    public void ItemSelectSlot2()
    {
        Item selected = _slot2[Random.Range(0, _slot2.Count)];
        Player.Instance.EquipItem(Instantiate(selected));
        ClosePopup();
    }

    public void ItemSelectSlot3()
    {
        Item selected = _slot3[Random.Range(0, _slot3.Count)];
        Player.Instance.EquipItem(Instantiate(selected));
        ClosePopup();
    }
    
    public void ClosePopup()
    {
        itemPopup.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
