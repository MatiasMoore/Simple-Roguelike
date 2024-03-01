using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ObjectMovement))]
public class Bullet : Projectile
{
    [SerializeField]
    private float _aliveTime;

    private float _timer;

    private ObjectMovement _objectMovement;

    private Vector2 _direction;

    public void Fire(Vector2 parentVelocity) 
    {
        _objectMovement = GetComponent<ObjectMovement>();
        _objectMovement.Init();

        if (parentVelocity == Vector2.zero)
        {       
            _direction = transform.right;
            return;
        }

        _direction = (Vector2)transform.right * _objectMovement.GetMaxSpeed();
        _direction += parentVelocity;
        _objectMovement.SetMaxSpeed(_direction.magnitude);
        _direction.Normalize();    
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        _objectMovement.SetDirection(_direction);
        if (_timer >= _aliveTime)
        {
            Destroy(gameObject);
        }
    }

    public ObjectMovement GetObjectMovement()
    {
        return _objectMovement;
    }

    public void SetAliveTime(float aliveTime)
    {
        _aliveTime = aliveTime;
    }

}
