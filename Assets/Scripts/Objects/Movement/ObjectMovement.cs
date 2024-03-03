using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ObjectMovement : MonoBehaviour
{
    [SerializeField]
    private float _accelerationTime = 0.3f;

    [SerializeField]
    private float _decelerationTime = 0.3f;

    [SerializeField]
    private float _changeDirectionTime = 0.15f;

    [SerializeField]
    private float _maxSpeed = 5f;

    private ObjectMovementMainState _objectMovementState;

    private Rigidbody2D _rigidbody;

    private Vector2 _direction;

    [SerializeField]
    private bool IsDynamicUpdateData = false;

    public enum WalkType
    {
        ByDirection,
        ByPoint
    }

    [SerializeField]
    private WalkType _walkType;

    // START OF DEBUG FIELDS: \\
    enum MovementState
    {
        Idle,
        Acceleration,
        Linear,
        Deceleration,
        ChangeDirection
    }

    private MovementState _debugStates = MovementState.Idle;
    private Vector2 _velocity;
    // END OF DEBUG FIELDS \\

    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _objectMovementState = new Idle(_rigidbody, _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed);
    }

    private void FixedUpdate()
    {
        if(IsDynamicUpdateData)
        {
            _objectMovementState.SetAccelerationTime(_accelerationTime);
            _objectMovementState.SetDecelerationTime(_decelerationTime);
            _objectMovementState.SetChangeDirectionTime(_changeDirectionTime);
            _objectMovementState.SetMaxSpeed(_maxSpeed);
        }

        _objectMovementState = _objectMovementState.Update(_direction, Time.fixedDeltaTime);
        UpdateDebug();
    }

    public void SetWalkType(WalkType walkType)
    {
        _walkType = walkType;
    }

    public void WalkTo(List<Vector3> path)
    {

        _objectMovementState = new PathFolowing(_rigidbody, _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed, path);
    }

    public void SetDirection(Vector2 direction)
    {
        if (_walkType == WalkType.ByDirection)
        {
            _direction = direction;
        }
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
        if (_objectMovementState != null)
        {
            _objectMovementState.SetMaxSpeed(maxSpeed);
        }
    }

    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }

    public void SetAccelerationTime(float accelerationTime)
    {
        _accelerationTime = accelerationTime;
        if (_objectMovementState != null)
        {
            _objectMovementState.SetAccelerationTime(accelerationTime);
        }
    }

    public float GetAccelerationTime()
    {
        return _accelerationTime;
    }

    public void SetDecelerationTime(float decelerationTime)
    {
        _decelerationTime = decelerationTime;
        if (_objectMovementState != null)
        {
            _objectMovementState.SetDecelerationTime(decelerationTime);
        }
    }

    public float GetDecelerationTime()
    {
        return _decelerationTime;
    }

    public void SetChangeDirectionTime(float changeDirectionTime)
    {
        _changeDirectionTime = changeDirectionTime;
        if (_objectMovementState != null)
        {
            _objectMovementState.SetChangeDirectionTime(changeDirectionTime);
        }
    }

    public float GetChangeDirectionTime()
    {
        return _changeDirectionTime;
    }

    public float GetCurrentSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }

    public void UpdateDebug()
    {

        if (_objectMovementState is Idle)
        {
            _debugStates = MovementState.Idle;
        }
        else if (_objectMovementState is Acceleration)
        {
            _debugStates = MovementState.Acceleration;
        }
        else if (_objectMovementState is Linear)
        {
            _debugStates = MovementState.Linear;
        }
        else if (_objectMovementState is Deceleration)
        {
            _debugStates = MovementState.Deceleration;
        }
        else if (_objectMovementState is ChangeDirection)
        {
            _debugStates = MovementState.ChangeDirection;
        } else if (_objectMovementState is PathFolowing)
        {
            _debugStates = MovementState.Linear;
        }

        _velocity = _rigidbody.velocity;
    }
}
