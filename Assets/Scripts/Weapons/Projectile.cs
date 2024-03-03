using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour
{
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private float _damage = 1;
    [SerializeField]
    private LayerMask _whatDestroysMe;
    [SerializeField]
    private LayerMask _whatIDamage;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_whatIDamage.value & 1 << collision.gameObject.layer) > 0)
        {
            IDamagable damagable = collision.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(_damage);
            }
        }
        
        
        if ((_whatDestroysMe.value & 1 << collision.gameObject.layer) > 0)
        {
            DestroyMe();
        }
    }

    public virtual void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public float GetDamage()
    {
        return _damage;
    }

    public void SetWhatIDamage(LayerMask whatIDamage)
    {
        _whatIDamage = whatIDamage;
    }

    public void SetWhatDestroysMe(LayerMask whatDestroysMe)
    {
        _whatDestroysMe = whatDestroysMe;
    }

    public LayerMask GetWhatIDamage()
    {
        return _whatIDamage;
    }

    public LayerMask GetWhatDestroysMe()
    {
        return _whatDestroysMe;
    }
}
