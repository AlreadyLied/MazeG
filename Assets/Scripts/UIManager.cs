using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [Header("Inventory")]
    public Image[] iconList = new Image[4];
    public GameObject[] slotList = new GameObject[4];
    public GameObject[] selects = new GameObject[4];

    [Header("Status")]
    public Image healthBar;
    public Image staminaBar;
    
    [Header("Screen Effects")]
    public CanvasGroup fadeOutCanvas;
    public CanvasGroup damagedCanvas;
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float redSpeed = 3f;
    [SerializeField] private float returnSpeed = 1.5f;
    private float redTemp;
    private float returnTemp;
    
    // DEBUG
    private int curIdx;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        curIdx = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) PlayerDamaged(10);
        if (Input.GetKeyDown(KeyCode.J)) PlayerDamaged(30);
        if (Input.GetKeyDown(KeyCode.K)) PlayerDamaged(50);
    }

    public void AddToInventory(Item item)
    {
        Image icon = iconList[item.itemIndex];
        icon.color = new Color(1f, 1f, 1f, 1f);
        icon.sprite = item.icon;
    }

    public void RemoveFromInventory(Item item)
    {
        Image icon = iconList[item.itemIndex];
        icon.sprite = null;
        icon.color = new Color(1f, 1f, 1f, 0f);
    }

    public void UpdateHealth(int health)
    {
        healthBar.fillAmount = health / 100;
    }

    public void UpdateStamina(float stamina)
    {
        staminaBar.fillAmount = stamina / 100;
    }

    public void PlayerDied()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (fadeOutCanvas.alpha < 1)
        {
            fadeOutCanvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    public void PlayerDamaged(int amount)
    {
        float damage;
        
        if (amount <= 10) damage = 0.2f;
        else if (amount <= 30) damage = 0.4f;
        else damage = 0.6f;
        redTemp = redSpeed * damage;
        returnTemp = returnSpeed * damage;

        StartCoroutine(Damaged(damage));
    }

    IEnumerator Damaged(float amount)
    {
        while (damagedCanvas.alpha < amount)
        {
            damagedCanvas.alpha += Time.deltaTime * redTemp;
            yield return null;
        }

        while (damagedCanvas.alpha > 0)
        {
            damagedCanvas.alpha -= Time.deltaTime * returnTemp;
            yield return null;
        }
    }

    public void ItemSelect(int nextIdx)
    {
        selects[curIdx].SetActive(false);
        selects[nextIdx].SetActive(true);
        curIdx = nextIdx;
    }
}
