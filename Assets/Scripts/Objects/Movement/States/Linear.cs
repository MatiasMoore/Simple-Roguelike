using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : ObjectMovementMainState
{
    private Vector2 _directionAtStart = Vector2.zero;
    public Linear(Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime, float maxSpeed) : base(rigidbody, accelerationTime, decelerationTime, changeDirectionTime, maxSpeed)
    {
    }

    public Linear(ObjectMovementMainState objectMovement) : base(objectMovement)
    {
    }

    public override ObjectMovementMainState Update(Vector2 direction, float deltaTime)
    {
        // Set the initial values for the linear movement
        if (_directionAtStart == Vector2.zero)
        {
            _directionAtStart = direction;
        }

        // Check Deceleration state
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
        GetRigidbody().velocity = GetMaxSpeed() * direction;

        return this;
    }
}