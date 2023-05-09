using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public GameObject[] iconList = new GameObject[4];

    public Image healthBar;
    public Image staminaBar;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
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
        healthBar.fillAmount = health;
    }

    public void UpdateStamina(float stamina)
    {
        staminaBar.fillAmount = stamina;
    }
}
