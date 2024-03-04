using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField]
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
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
            
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

    public void Stop()
    {
        //destroy current state
        _objectMovementState = new Idle(_rigidbody, _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed);
        _direction = Vector2.zero;
    }

    public void GoToPointOnNavMesh(Vector2 point)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath((Vector2)transform.position, point, NavMesh.AllAreas, path))
        {
            WalkTo(path.corners.ToList());
            _debugPath = path.corners.ToList();
        } else
        {
            NavMeshHit hit = new NavMeshHit();
            NavMesh.SamplePosition((Vector2)transform.position, out hit, 9999, NavMesh.AllAreas);
            if (NavMesh.CalculatePath(hit.position, point, NavMesh.AllAreas, path))
            {
                List<Vector3> pathList = new List<Vector3>();
                pathList.Add(Vector3.zero);
                pathList.AddRange(path.corners.ToList());
                WalkTo(pathList);
                _debugPath = pathList;
            }

        }
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

    private List<Vector3> _debugPath = new List<Vector3>();
    private void OnDrawGizmos()
    {
        foreach (var pos in _debugPath)
        {
            DebugDraw.DrawCross(pos, 0.2f, Color.green);
        }
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
