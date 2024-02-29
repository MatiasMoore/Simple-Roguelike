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

    public void Start()
    {
        _objectMovement = GetComponent<ObjectMovement>();
        _objectMovement.Init();
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        _objectMovement.SetDirection(transform.right);
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
