using UnityEngine;
using System.Collections;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private float _trapDuration = 2.5f;
    [SerializeField] private int _trapDamage = 10;
    [SerializeField] private int _tickDamage = 2;
    [SerializeField] private int _tickCount = 10;

    private Collider _collider;
    private Animator _anim;
    private readonly int _disposeTriggerHash = Animator.StringToHash("dispose");

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        _collider.enabled = false;

        Player.Instance.TakeDamage(_trapDamage);
        Player.Instance.Bleed(_tickDamage, _tickCount);
        Player.Movement.Bind(_trapDuration, transform.position, 0.075f);

        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        _anim.enabled = true;

        yield return new WaitForSeconds(_trapDuration);

        _anim.SetTrigger(_disposeTriggerHash);

        yield return new WaitForSeconds(1.5f); // time for animation to complete

        Destroy(gameObject);
    }
}
