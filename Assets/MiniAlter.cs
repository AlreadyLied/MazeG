using UnityEngine;

public class MiniAlter : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject _fragment;

    public void OnInteract()
    {
        if (_fragment == null) return;

        print("hi");
        Destroy(_fragment);
        Player.Items.fragmentCount++;
    }
}
