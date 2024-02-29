using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectMovementMainState
{
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private float _accelerationTime = 0.1f;
    private float _decelerationTime = 0.1f;
    private float _changeDirectionTime = 0.1f;
    private float _maxSpeed = 5f;
    private LayerMask _obstacleMask;

    public ObjectMovementMainState( Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime,  float maxSpeed, LayerMask obstacleMask, Collider2D collider)
    {
        _rigidbody = rigidbody;
        _accelerationTime = accelerationTime;
        _decelerationTime = decelerationTime;
        _changeDirectionTime = changeDirectionTime;
        _maxSpeed = maxSpeed;
        _obstacleMask = obstacleMask;
        _collider = collider;
    }

    public ObjectMovementMainState(ObjectMovementMainState objectMovement)
    {
        _rigidbody = objectMovement.GetRigidbody();
        _accelerationTime = objectMovement.GetAccelerationTime();
        _decelerationTime = objectMovement.GetDecelerationTime();
        _changeDirectionTime = objectMovement.GetChangeDirectionTime();
        _maxSpeed = objectMovement.GetMaxSpeed();
        _obstacleMask = objectMovement.GetObstacleMask();
        _collider = objectMovement.GetCollider();
    }
    public abstract ObjectMovementMainState Update(Vector2 direction, float deltaTime);

    protected Vector2 AdjustVelocityByObstacles(Vector2 velocity, Vector2 direction)
    {
        velocity.x = IsObstacleAt(new Vector2(Mathf.RoundToInt(direction.x), 0), 0.1f) ? 0 : velocity.x;
        velocity.y = IsObstacleAt(new Vector2(0, Mathf.RoundToInt(direction.y)), 0.1f) ? 0 : velocity.y;
        
        return velocity;
    }

    protected bool IsObstacleAt(Vector2 direction, float distance)
    {
        // Cast Ray
        Vector2 raycastOrigin = (Vector2)GetRigidbody().transform.position + direction * _collider.bounds.extents;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, direction, distance, _obstacleMask);
        
        // Draw ray
        Vector2 directionToDraw = direction * distance;
        Debug.DrawRay(raycastOrigin, directionToDraw, Color.red);

        return hit.collider != null;
    }

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

    public void SetObstacleMask(LayerMask obstacleMask)
    {
        _obstacleMask = obstacleMask;
    }

    public LayerMask GetObstacleMask()
    {
           return _obstacleMask;
    }

    public Collider2D GetCollider()
    {
        return _collider;
    }

    public void SetCollider(Collider2D collider)
    {
        _collider = collider;
    }
}
