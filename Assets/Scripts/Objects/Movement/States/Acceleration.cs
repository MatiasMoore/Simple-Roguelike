using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acceleration : ObjectMovementMainState
{
    private float _timer = 0f;

    private Vector2 _velocityAtStart;

    private Vector2 _targetVelocity = Vector2.zero;

    private Vector2 _directionAtStart = Vector2.zero;

    public Acceleration(ObjectMovementMainState objectMovement) : base(objectMovement)
    {
    }

    public override ObjectMovementMainState Update(Vector2 direction, float deltaTime)
    {
        // Set the initial values for the acceleration
        if (_directionAtStart == Vector2.zero)
        {
            SetUpInitialValues(direction);
        }

        _timer += deltaTime;

        // Check Deceleartion state
        if (direction == Vector2.zero)
        {
            return new Deceleration(this);
        }

        // Check ChangeDirection state
        if (direction != _directionAtStart)
        {
            ChangeDirection changeDirection = new ChangeDirection(this);
            changeDirection.Update(direction, deltaTime);
            return changeDirection;
        }

        // Moving
        if (_timer < GetAccelerationTime())
        {
            Vector2 velocity = InterpolateVector(_velocityAtStart, _targetVelocity, _timer / GetAccelerationTime());
            GetRigidbody().AddForce((velocity - GetRigidbody().velocity) * GetRigidbody().mass / deltaTime);
            return this;
        }
                
        return new Linear(this);
    }

    private void SetUpInitialValues(Vector2 direction)
    {
        _directionAtStart = direction;
        _velocityAtStart = GetRigidbody().velocity;
        _targetVelocity = GetMaxSpeed() * direction;
        _timer = GetAccelerationTime() - (_targetVelocity - _velocityAtStart).magnitude / GetMaxSpeed() * GetAccelerationTime();
    }

    private Vector2 InterpolateVector(Vector2 start, Vector2 end, float timeState)
    {
        return start + (end - start) * EaseInQuad(timeState);
    }

    private float EaseInQuad(float x)
    {
        return 1 - (float)Math.Cos((x * Math.PI) / 2);
    }

}
