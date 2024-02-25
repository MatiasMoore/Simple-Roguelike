using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : ObjectMovement
{
    public Idle(Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime, float maxSpeed) : base(rigidbody, accelerationTime, decelerationTime, changeDirectionTime, maxSpeed)
    {
    }

    public Idle(ObjectMovement objectMovement) : base(objectMovement)
    {
    }

    public override ObjectMovement Update(Vector2 direction, float deltaTime)
    {
        // Check Acceleration state
        if (direction != Vector2.zero)
        {
           return new Acceleration(this);
        }

        // Stay
        GetRigidbody().velocity = Vector2.zero;

        return this;
    }
}
