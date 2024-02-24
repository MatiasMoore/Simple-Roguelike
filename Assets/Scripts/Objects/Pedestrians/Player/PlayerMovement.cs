using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : Pedestrian
{
    private Rigidbody2D _rigidbody;

    private InputSystem _inputSystem;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _accelerationTime = 0.1f;
    [SerializeField]
    private float _decelerationTime = 0.1f;
    [SerializeField]
    private float _changeDirectionTime = 5f;
    [SerializeField]
    private float _maxSpeed = 5f;

    private ObjectMovement _objectMovement;

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
    private Vector3 _direction;
    // START END OF DEBUG FIELDS: \\

    private const string _speedX = "SpeedX";
    private const string _speedY = "SpeedY";
    private const string _cursorX = "CursorX";
    private const string _cursorY = "CursorY";
    
    public void Init(InputSystem inputSystem)
    { 
        _rigidbody = GetComponent<Rigidbody2D>();
        _objectMovement = new Idle(_rigidbody, _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed);
        _inputSystem = inputSystem;
    }

    private void Update()
    {
        if (_inputSystem != null)
        {
            _animator.SetFloat(_speedX, InputSystem.Movement.x);
            _animator.SetFloat(_speedY, InputSystem.Movement.y);
            
            //translate cursor position to scene position
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(InputSystem.CursorPosition);

            _animator.SetFloat(_cursorX, cursorPosition.x - transform.position.x);
            _animator.SetFloat(_cursorY, cursorPosition.y - transform.position.y);

            _objectMovement = _objectMovement.Update(InputSystem.Movement, Time.deltaTime);
            UpdateDebug();
        } 
    }

    private void UpdateDebug()
    {
        
       if (_objectMovement is Idle)
        {
           _debugStates = MovementState.Idle;
       }
       else if (_objectMovement is Acceleration)
        {
           _debugStates = MovementState.Acceleration;
       }
       else if (_objectMovement is Linear)
        {
           _debugStates = MovementState.Linear;
       }
       else if (_objectMovement is Deceleration)
        {
           _debugStates = MovementState.Deceleration;
       } else if (_objectMovement is ChangeDirection)
       {
           _debugStates = MovementState.ChangeDirection;
       }

        _direction = InputSystem.Movement;
    }

}
