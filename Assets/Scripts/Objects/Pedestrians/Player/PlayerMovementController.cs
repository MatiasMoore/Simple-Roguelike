using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ObjectMovement))]
public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private InputSystem _inputSystem;

    [SerializeField]
    private Animator _animator;

    private ObjectMovement _objectMovement;

    // START OF DEBUG FIELDS: \\

    private Vector2 _direction;
    
    // END OF DEBUG FIELDS: \\

    private const string _speedX = "SpeedX";
    private const string _speedY = "SpeedY";
    private const string _cursorX = "CursorX";
    private const string _cursorY = "CursorY";
    
    public void Init(InputSystem inputSystem)
    { 
        _objectMovement = GetComponent<ObjectMovement>();
        _objectMovement.Init();
        _inputSystem = inputSystem;
    }

    private void Update()
    {
        if (_inputSystem != null)
        {
            _animator.SetFloat(_speedX, InputSystem.Movement.x);
            _animator.SetFloat(_speedY, InputSystem.Movement.y);
                 

            _animator.SetFloat(_cursorX, InputSystem.CursorPosition.x - transform.position.x);
            _animator.SetFloat(_cursorY, InputSystem.CursorPosition.y - transform.position.y);

            _objectMovement.SetDirection(new Vector2(InputSystem.Movement.x, InputSystem.Movement.y));
            UpdateDebug();
        } 
    }

    private void UpdateDebug()
    {
        _direction = InputSystem.Movement;
    }

}
