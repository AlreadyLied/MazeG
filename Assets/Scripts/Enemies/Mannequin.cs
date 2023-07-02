using UnityEngine;

[RequireComponent(typeof(MannequinLOSDetect))]
public class Mannequin : MonoBehaviour
{
    [SerializeField] private int _detectFrameInterval = 100;

    private MannequinLOSDetect _losDetect;
    private bool _isInSight;

    private void Start()
    {
        _losDetect = GetComponent<MannequinLOSDetect>();
    }

    private void Update()
    {
        if (Time.frameCount % _detectFrameInterval == 0)
        {
            if (_losDetect.InPlayerSight())
            {
                // On player sight enter
                if (!_isInSight)
                {
                    _isInSight = true;

                    print("sight enter");
                }
            }
            else
            {
                // On player sight exit
                if (_isInSight)
                {
                    _isInSight = false;

                    print("sight exit");
                }

                transform.LookAtHeightCorrected(Player.Transform);
            }
        }
    }
}
