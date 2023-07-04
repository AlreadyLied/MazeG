using UnityEngine;

public class Altar : MonoBehaviour, Interactable
{
    private int _fragmentCount;

    public void OnInteract()
    {
        if (Player.Items.fragmentCount > 0)
        {
            _fragmentCount += Player.Items.fragmentCount;
            Player.Items.fragmentCount = 0;
        }
    }
}
