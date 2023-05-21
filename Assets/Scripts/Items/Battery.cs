using UnityEngine;

public class Battery : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        Player.Flashlight.ChargeBattery(20f);
        Destroy(gameObject);
        print("Battery Charged\n");
    }
}
