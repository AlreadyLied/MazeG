using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public GameObject[] iconList = new GameObject[4];
    public Material healthBar;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        healthBar.SetFloat("_FILL", 0.1f);
    }

    public void AddToInventory(Item item)
    {
        GameObject icon = iconList[item.itemInedx];
        icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        icon.GetComponent<Image>().sprite = item.icon;
    }

    public void RemoveFromInventory(Item item)
    {
        GameObject icon = iconList[item.itemInedx];
        icon.GetComponent<Image>().sprite = null;
        icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    public void UpdateHealth(float health)
    {
        healthBar.SetFloat("_FILL", health);
    }

    public void UpdateStamina(float stamina)
    {
        
    }

    private void Update()
    {
        // This Update method is only used for DEBUG
        if (Input.GetKeyDown(KeyCode.H))
        {
            UpdateHealth(0.7f);
        }
    }
}
