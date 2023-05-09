using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    
    public GameObject itemPopup;
    
    public Item healthPack;
    public List<Item> slot2;
    public List<Item> slot3;

    void Awake()
    {
        instance = this;
    }

    public void ShowItems()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        itemPopup.SetActive(true);
    }

    public void ItemSelectSlot1()
    {
        Player.Items.AddItem(Instantiate(healthPack));
        ClosePopup();
    }

    public void ItemSelectSlot2()
    {
        Item selected = slot2[Random.Range(0, slot2.Count)];
        Player.Items.AddItem(Instantiate(selected));
        ClosePopup();
    }

    public void ItemSelectSlot3()
    {
        Item selected = slot3[Random.Range(0, slot3.Count)];
        Player.Items.AddItem(Instantiate(selected));
        ClosePopup();
    }
    
    public void ClosePopup()
    {
        itemPopup.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
