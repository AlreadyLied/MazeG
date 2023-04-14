using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    void Awake()
    {
        instance = this;
    }

    public GameObject healthPack;
    public GameObject adrenalineShot;

    public GameObject itemPopup;

    public void ShowItems()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        itemPopup.SetActive(true);
    }

    public void ItemSelectSlot1()
    {
        // Player.Instance.EquipItem(Instantiate(healthPack));
        ClosePopup();
    }

    public void ItemSelectSlot2()
    {
        // Player.Instance.EquipItem(Instantiate(adrenalineShot));
        ClosePopup();
    }

    public void ItemSelectSlot3()
    {
        ClosePopup();
    }
    
    public void ClosePopup()
    {
        itemPopup.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
