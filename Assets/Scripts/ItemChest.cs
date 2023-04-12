using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ItemChest : MonoBehaviour, Interactable
{
    public Transform itemPopup;
    
    public void OnInteract()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        itemPopup.gameObject.SetActive(true);
    }

    public void ItemSelect()
    {
        itemPopup.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
