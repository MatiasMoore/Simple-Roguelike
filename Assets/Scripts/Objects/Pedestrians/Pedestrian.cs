using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Pedestrian : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float _hp;

    [SerializeField]
    private float _maxHp;

    [SerializeField]
    private HealthBar _healthBar;

    public UnityAction OnDeath;

    private void Start()
    {
        _hp = _maxHp;
        _healthBar.UpdateHealth(_hp, _maxHp);
    }

    public void TakeDamage(float damage)
    {
        if (_hp > 0)
        {
            _hp -= damage;
            _healthBar.SetCurrenthealth(_hp);
        }

        if (_hp <= 0)
        {
            Die();
        }
    }

    public void SetHp(int hp)
    {
        _hp = hp;
        _healthBar.SetCurrenthealth(_hp);
    }

    public void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
        _healthBar.SetMaxHealth(_maxHp);
    }

    public float GetHp()
    {
        return _hp;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"Pedestrian {gameObject.name} died");
        Destroy(gameObject);
    }

}
