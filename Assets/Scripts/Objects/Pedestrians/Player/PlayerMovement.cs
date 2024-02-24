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

    private const string _speedX = "SpeedX";
    private const string _speedY = "SpeedY";
    private const string _cursorX = "CursorX";
    private const string _cursorY = "CursorY";
    
    public void Init(InputSystem inputSystem)
    { 
        _rigidbody = GetComponent<Rigidbody2D>();
        _inputSystem = inputSystem;
    }

    private void Update()
    {
        if (_inputSystem != null)
        {
            Move(InputSystem.Movement);
            _animator.SetFloat(_speedX, InputSystem.Movement.x);
            _animator.SetFloat(_speedY, InputSystem.Movement.y);
            
            //translate cursor position to scene position
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(InputSystem.CursorPosition);

            _animator.SetFloat(_cursorX, cursorPosition.x - transform.position.x);
            _animator.SetFloat(_cursorY, cursorPosition.y - transform.position.y);
        } 
    }

    private void Move(Vector2 direction)
    {
        
        _rigidbody.velocity = direction * GetSpeed();
        Debug.Log($"direction {direction}, velocity {_rigidbody.velocity}");
    }

}
