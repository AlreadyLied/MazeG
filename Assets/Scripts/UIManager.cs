using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    
    public GameObject itemPopup;

    public void ShowItems()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        itemPopup.SetActive(true);
    }

    public void ItemSelect()
    {
        itemPopup.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
