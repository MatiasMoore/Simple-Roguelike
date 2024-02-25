using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField]
    private float _accelerationTime;

    [SerializeField]
    private float _decelerationTime;

    [SerializeField]
    private float _changeDirectionTime;

    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private float _aliveTime;

    private float _timer;

    private ObjectMovement _objectMovement;

    public void Start()
    {
        _objectMovement = new Idle(GetComponent<Rigidbody2D>(), _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed);
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        _objectMovement = _objectMovement.Update(transform.right, Time.deltaTime);
        if (_timer >= _aliveTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetAccelerationTime(float accelerationTime)
    {
        _accelerationTime = accelerationTime;
    }

    public void SetDecelerationTime(float decelerationTime)
    {
        _decelerationTime = decelerationTime;
    }

    public void SetChangeDirectionTime(float changeDirectionTime)
    {
        _changeDirectionTime = changeDirectionTime;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
    }

    public void SetAliveTime(float aliveTime)
    {
        _aliveTime = aliveTime;
    }

}
