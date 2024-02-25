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

    public UnityAction OnDeath;

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Die();
        }
    }

    public void SetHp(int hp)
    {
        _hp = hp;
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
