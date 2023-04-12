using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemChest : MonoBehaviour
{
    public GameObject ItemPopup;

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         Cursor.visible = true;
    //         Cursor.lockState = CursorLockMode.Confined;
    //         ItemPopup.SetActive(true);
    //     }
    // }

    public void ItemSelect()
    {
        ItemPopup.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
