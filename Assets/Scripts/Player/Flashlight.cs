using UnityEngine;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float _batteryDrainRate = 0.25f;

    private float _batteryLeft = 100f;
    private float _initialLightIntensity;
    private Coroutine _flashRoutine;
    
    public float battery { get { return _batteryLeft; } }
    public bool isOn { get; private set; }

    private void Start()
    {
        _initialLightIntensity = _light.intensity;
        _light.enabled = false;
    }

    public void Toggle()
    {
        if (isOn)
        {
            ToggleOff();
        }
        else if (_batteryLeft > 0f)
        {
            ToggleOn();
        }
    }

    public void ChargeBattery(float delta)
    {
        _batteryLeft += delta;
    }

    private void ToggleOn()
    {
        _light.enabled = true;
        isOn = true;

        _flashRoutine = StartCoroutine(FlashRoutine());
    }
    
    private void ToggleOff()
    {
        _light.enabled = false;
        isOn = false;

        StopCoroutine(_flashRoutine);
    }
    
    private IEnumerator FlashRoutine()
    {
        while (_batteryLeft > 0f)
        {
            if (_batteryLeft > 100f)
            {
                _light.intensity = _initialLightIntensity;
            }
            else
            {
                float interpolation = -Mathf.Pow(((_batteryLeft / 100) - 1), 4) + 1;
                float noise = Mathf.Pow(Random.value * (1 - interpolation), 2); // gets more noisy as battery runs out

                _light.intensity = _initialLightIntensity * interpolation + noise;
            }

            _batteryLeft -= _batteryDrainRate * Time.deltaTime;

            yield return null;
        }

        _batteryLeft = 0f;
        ToggleOff();
    }
}
