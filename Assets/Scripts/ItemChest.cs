using System;
using System.Collections;
using UnityEngine;

public class ItemChest : MonoBehaviour, Interactable
{
    public void OnInteract()
    {
        UIManager.instance.ShowItems();
        Destroy(gameObject);
    }
}
