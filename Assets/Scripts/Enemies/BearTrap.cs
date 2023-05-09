using UnityEngine;
using System.Collections;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private float _trapDuration = 2.5f;
    [SerializeField] private int _trapDamage = 10;
    [SerializeField] private int _tickDamage = 2;
    [SerializeField] private int _tickCount = 10;

    public System.Action OnActivated;
    public bool trapActive { get; private set; } = false;

    private Collider _collider;
    private Animator _anim;
    private readonly int _disposeTriggerHash = Animator.StringToHash("dispose");

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _anim = GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        _collider.enabled = false;

        Player.Health.TakeDamage(_trapDamage);
        Player.Health.Bleed(_tickDamage, _tickCount);
        Player.Movement.Bind(_trapDuration, transform.position, 0.075f);

        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        _anim.enabled = true;
        _anim.Rebind();
        _anim.Update(0f);

        yield return new WaitForSeconds(_trapDuration);

        _anim.SetTrigger(_disposeTriggerHash);

        yield return new WaitForSeconds(1.5f); // time for animation to complete

        gameObject.SetActive(false);
        trapActive = false;
        OnActivated?.Invoke();
    }

    public void Enable() // for pooling - 
    {
        trapActive = true;
        gameObject.SetActive(true);
        _collider.enabled = true;
    }

}
