using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public abstract void Use();

    public void Used()
    {
        UIManager.instance.RemoveFromInventory(this);
    }
    
    public Sprite icon;
    public int itemInedx;
}
