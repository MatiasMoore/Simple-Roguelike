using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deceleration : ObjectMovementMainState
{
    private float _timer = 0f;

    private Vector2 _velocityAtStart;

    public Deceleration(ObjectMovementMainState objectMovement) : base(objectMovement)
    {
        _velocityAtStart = GetRigidbody().velocity;
        _timer = GetDecelerationTime() - _velocityAtStart.magnitude / GetMaxSpeed() * GetDecelerationTime();
    }   

    public override ObjectMovementMainState Update(Vector2 direction, float deltaTime)
    {
        _timer += deltaTime;

        // Check Acceleration state
        if (direction != Vector2.zero)
        {
            ChangeDirection changeDirection  = new ChangeDirection(this);
            changeDirection.Update(direction, deltaTime);
            return changeDirection;
        }

        // Moving
        if (_timer < GetDecelerationTime())
        {
            Vector2 velocity = InterpolateVector(_velocityAtStart, Vector2.zero, _timer / GetDecelerationTime());       
            GetRigidbody().AddForce((velocity - GetRigidbody().velocity) * GetRigidbody().mass / deltaTime);
            return this;
        }

        return new Idle(this);
    }
    private Vector2 InterpolateVector(Vector2 start, Vector2 end, float timeState)
    {
        return start + (end - start) * EaseOutQuad(timeState);
    }

    private float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }

}
