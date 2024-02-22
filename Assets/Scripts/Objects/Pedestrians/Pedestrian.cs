using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Pedestrian : MonoBehaviour
{
    [SerializeField]
    private float _hp;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float damage;

    public UnityEvent OnDeath;
    //TODO: Weapon

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Die();
        }
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
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
    }

}
