using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private int _health = 100;

    [SerializeField]
    private int _bleedIntervalSeconds = 1;
    private int _bleedPerTick;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            UIManager.instance.PlayerDied();
        }
        UIManager.instance.UpdateHealth(_health);
    }

    public void Heal(int health)
    {
        _health = Mathf.Min(_health + health, 100);
        UIManager.instance.UpdateHealth(_health);
    }

    public void Bleed(int tickDamage, int tickCount)
    {
        StartCoroutine(BleedRoutineHelper(tickDamage, tickCount));

        if (_bleedPerTick == tickDamage)
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

    private IEnumerator BleedRoutine()
    {
        while (_bleedPerTick != 0)
        {
            TakeDamage(_bleedPerTick);

            yield return new WaitForSeconds(_bleedIntervalSeconds);
        }
    }
}
