using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private int _health = 100;

    [SerializeField]
    private int _bleedIntervalSeconds = 1;
    private int _bleedPerTick;

    private void OnDisable() => StopAllCoroutines();

    public void TakeDamage(int damage)
    {
        if (_health == 0) return; // Already dead

        _health = Mathf.Max(_health - damage, 0);

        UIManager.instance.UpdateHealth(_health);
        UIManager.instance.PlayerDamaged(damage);

        if (_health == 0)
        {
            Player.Instance.OnDeath();
        }
    }

    public void Heal(int health)
    {
        _health = Mathf.Min(_health + health, 100);
        UIManager.instance.UpdateHealth(_health);
    }

    public void Bleed(int tickDamage, int tickCount)
    {
        StartCoroutine(BleedRoutineHelper(tickDamage, tickCount));

        if (_bleedPerTick == tickDamage) // * If _bleedPerTick was 0 prior to calling BleedRoutineHelper
        {
            StartCoroutine(BleedRoutine());
        }
    }

    private IEnumerator BleedRoutineHelper(int tickDamage, int tickCount)
    {
        _bleedPerTick += tickDamage;

        yield return new WaitForSeconds(tickCount * _bleedIntervalSeconds);

        _bleedPerTick -= tickDamage;
    }

    private IEnumerator BleedRoutine() // * Applies _bleedPerTick damage every _bleedIntervalSeconds
    {
        while (_bleedPerTick != 0)
        {
            TakeDamage(_bleedPerTick);

            yield return new WaitForSeconds(_bleedIntervalSeconds);
        }
    }
}
