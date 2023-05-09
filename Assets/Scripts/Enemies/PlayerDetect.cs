using UnityEngine;

public class PlayerDetect : MonoBehaviour
{   
    [Header("Detection Options")]
    [SerializeField] private float _detectDistance = 4f;
    [Range(0, 360)]
    [SerializeField] private float _detectAngle = 120f; 
    [SerializeField] private int _detectInterval = 10; // Interval in frames to check detection
    [SerializeField] private Transform _detectRayOriginTr;
    
    private int _obstructionlayer = (1 << (int)Layer.Wall);

    public delegate void DetectEvent();

    private DetectEvent OnPlayerDetectEnter;
    private DetectEvent OnPlayerDetectExit;
    public bool detected { get; private set; }

    // Register callback methods
    public void Register(DetectEvent enter, DetectEvent exit)
    {
        OnPlayerDetectEnter = enter;
        OnPlayerDetectExit = exit;
    }

    private void Update()
    {
        if (Time.frameCount % _detectInterval == 0) 
        {
            if (PlayerInSight())
            {
                if (!detected) // detect enter
                {
                    OnPlayerDetectEnter?.Invoke();
                    detected = true;
                }
            }
            else
            {
                if (detected) // detect exit
                {
                    OnPlayerDetectExit?.Invoke();
                    detected = false;
                }
            }
        }
    }

    private bool PlayerInSight()
    {
        Vector3 dirToPlayer = Player.Position - _detectRayOriginTr.position;
        dirToPlayer.y = 0;

        float distance = dirToPlayer.magnitude;

        if (distance > _detectDistance) return false;

        if (Vector3.Angle(_detectRayOriginTr.forward, dirToPlayer) > _detectAngle / 2) return false;

        // TODO: more rays
        if (Physics.Raycast(_detectRayOriginTr.position, dirToPlayer, distance, _obstructionlayer)) return false;

        return true;
    }
}
