using System;
using System.Collections;
using UnityEngine;

public class ItemChest : MonoBehaviour, Interactable
{
    public void OnInteract()
    {
        ItemManager.instance.ShowItems();
        Destroy(gameObject);
    }
}
