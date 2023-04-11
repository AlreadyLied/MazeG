using UnityEngine;

public interface Item
{
    public void Primary();
    public void Secondary();
}

public class Syringe : MonoBehaviour, Item
{
    [SerializeField] private int _healthAddAmount;

    public void Primary()
    {
        Player.Instance.Heal(_healthAddAmount);
        print("hi");
        Destroy(gameObject);
    }
    
    public void Secondary()
    {

    }
}
