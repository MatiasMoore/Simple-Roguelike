using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Pedestrian : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float _hp;

    [SerializeField]
    private float _maxHp;

    [SerializeField]
    private SliderManager _healthBar;

    [SerializeField]
    private bool _destroyOnDeath = true;

    public UnityAction OnDeath;

    private void Start()
    {
        _hp = _maxHp;
        _healthBar.UpdateValues(_hp, _maxHp);
    }

    public void TakeDamage(float damage)
    {
        if (_hp > 0)
        {
            _hp -= damage;
            _healthBar.SetCurrentValue(_hp);
        }

        if (_hp <= 0)
        {
            Die();
        }
    }

    public void SetHp(int hp)
    {
        _hp = hp;
        _healthBar.SetCurrentValue(_hp);
    }

    public void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
        _healthBar.SetMaxValue(_maxHp);
    }

    public float GetHp()
    {
        return _hp;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"Pedestrian {gameObject.name} died");
        if (_destroyOnDeath)
            Destroy(gameObject);
    }

}
