using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirection : ObjectMovement
{
    private float _timer = 0f;

    private Vector2 _velocityAtStart;

    private Vector2 _targetVelocity = Vector2.zero;

    private Vector2 _directionAtStart = Vector2.zero;

    public ChangeDirection(Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime, float maxSpeed) : base(rigidbody, accelerationTime, decelerationTime, changeDirectionTime, maxSpeed)
    {

    }

    public ChangeDirection(ObjectMovement objectMovement) : base(objectMovement)
    {

    }
    public override ObjectMovement Update(Vector2 direction, float deltaTime)
    {
        _timer += deltaTime;

        // Set the initial values for the change direction
        if (_directionAtStart == Vector2.zero)
        {
            SetUpInitialValues(direction);
        }
        
        // Check Deceleration state
        if (direction == Vector2.zero)
        {
            return new Deceleration(this);
        }

        // Check new direction for ChangeDirection state
        if (direction != _directionAtStart)
        {
            ChangeDirection changeDirection = new ChangeDirection(this);
            changeDirection.Update(direction, deltaTime);
            return changeDirection;
        }

        if (_timer < GetChangeDirectionTime())
        {          
            GetRigidbody().velocity = InterpolateVector(_velocityAtStart, _targetVelocity, _timer / GetChangeDirectionTime());
            return this;
        }

        return new Linear(this);
    }
    private void SetUpInitialValues(Vector2 direction)
    {
        _velocityAtStart = GetRigidbody().velocity;
        _directionAtStart = direction;
        _targetVelocity = GetMaxSpeed() * direction;
    }

    private Vector2 InterpolateVector(Vector2 start, Vector2 end, float timeState)
    {
        return start + (end - start) * Cubic(timeState);
    }

    private float Cubic(float x)
    {
        return x*x*x;
    }


}
