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
    public CanvasGroup fadeOutCanvas;
    private float fadeSpeed = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    public void AddToInventory(Item item)
    {
        GameObject icon = iconList[item.itemIndex];
        icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        icon.GetComponent<Image>().sprite = item.icon;
    }

    public void RemoveFromInventory(Item item)
    {
        GameObject icon = iconList[item.itemIndex];
        icon.GetComponent<Image>().sprite = null;
        icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    public void UpdateHealth(float health)
    {
        healthBar.fillAmount = health / 100;
    }

    public void UpdateStamina(float stamina)
    {
        staminaBar.fillAmount = stamina / 100;
    }

    public void PlayerDied()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        while (fadeOutCanvas.alpha < 1)
        {
            fadeOutCanvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
