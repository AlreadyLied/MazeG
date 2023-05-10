using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    // TEMP
    [SerializeField] private Text _batteryLeftText;

    [SerializeField] private Light _light;
    // [SerializeField] private float _flashHitDistance = 10f;
    [SerializeField] private float _batteryDrainRate = 0.25f;

    private float _batteryLeft = 100f;
    private float _initialLightIntensity;
    private Coroutine _flashRoutine;
    private bool _isOn;

    // private int _beamCollideLayer = (1 << (int)Layer.Wall) | (1 << (int)Layer.Enemy);

    // TEMP
    private void SetBatteryText()
    {
        // _batteryLeftText.text = $"Battery Left: {_batteryLeft:.00}";
    }

    private void Start()
    {
        _initialLightIntensity = _light.intensity;
        _light.enabled = false;
        
        SetBatteryText();
    }

    public void Toggle()
    {
        if (_isOn)
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
        SetBatteryText();
    }

    private void ToggleOn()
    {
        _light.enabled = true;
        _isOn = true;

        _flashRoutine = StartCoroutine(FlashRoutine());
    }
    

    private void ToggleOff()
    {
        _light.enabled = false;
        _isOn = false;

        StopCoroutine(_flashRoutine);
    }
    
    // drain battery and check ghost 
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
            SetBatteryText();


            // if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _flashHitDistance, _beamCollideLayer, QueryTriggerInteraction.Collide))
            // {
            //     GameObject hitObject = hit.collider.gameObject;

            //     if (hitObject.layer == (int)Layer.Enemy)
            //     {
            //         // TEMP
            //         if (hitObject.TryGetComponent(out GhostTest gt))
            //         {
            //             // gt.Hurt();
            //         }
            //     }
            // }


            yield return null;
        }

        _batteryLeft = 0f;
        SetBatteryText();
        ToggleOff();
    }
}
