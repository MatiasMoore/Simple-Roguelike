using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectMovementMainState
{
    private Rigidbody2D _rigidbody;

    private float _accelerationTime = 0.1f;
    private float _decelerationTime = 0.1f;
    private float _changeDirectionTime = 0.1f;
    private float _maxSpeed = 5f;

    public ObjectMovementMainState( Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime,  float maxSpeed)
    {
        _rigidbody = rigidbody;
        _accelerationTime = accelerationTime;
        _decelerationTime = decelerationTime;
        _changeDirectionTime = changeDirectionTime;
        _maxSpeed = maxSpeed;
    }

    public ObjectMovementMainState(ObjectMovementMainState objectMovement)
    {
        _rigidbody = objectMovement.GetRigidbody();
        _accelerationTime = objectMovement.GetAccelerationTime();
        _decelerationTime = objectMovement.GetDecelerationTime();
        _changeDirectionTime = objectMovement.GetChangeDirectionTime();
        _maxSpeed = objectMovement.GetMaxSpeed();     
    }
    public abstract ObjectMovementMainState Update(Vector2 direction, float deltaTime);

    public void SetMaxSpeed(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
    }

    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }

    public void SetAccelerationTime(float accelerationTime)
    {
        _accelerationTime = accelerationTime;
    }

    public float GetAccelerationTime()
    {
        return _accelerationTime;
    }

    public void SetDecelerationTime(float decelerationTime)
    {
        _decelerationTime = decelerationTime;
    }

    public float GetDecelerationTime()
    {
        return _decelerationTime;
    }

    public void SetRigidbody(Rigidbody2D rigidbody)
    {
        _rigidbody = rigidbody;
    }

    public Rigidbody2D GetRigidbody()
    {
        return _rigidbody;
    }

    public void SetChangeDirectionTime(float changeDirectionTime)
    {
        _changeDirectionTime = changeDirectionTime;
    }

    public float GetChangeDirectionTime()
    {
        return _changeDirectionTime;
    }
}
