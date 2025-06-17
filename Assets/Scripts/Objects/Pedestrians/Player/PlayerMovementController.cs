using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectMovement))]
public class PlayerMovementController : MonoBehaviour
{
    public bool IsDashUpgraded = false;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Animator _hatAnimator;

    private ObjectMovement _objectMovement;

    public float _dashSpeed;

    public float _dashDuration;

    public float _dashCooldown;

    private float _currentDashCooldown;
    // START OF DEBUG FIELDS: \\

    private Vector2 _direction;

    // END OF DEBUG FIELDS: \\

    private const string _moveSpeed = "MoveSpeed";
    private const string _cursorX = "CursorX";
    private const string _cursorY = "CursorY";
    
    public void Init(InputSystem inputSystem)
    { 
        _objectMovement = GetComponent<ObjectMovement>();
        _objectMovement.Init();

        InputSystem.Instance.DashEvent += Dash;
    }

    private void Update()
    {
        if (InputSystem.Instance != null)
        {
            _animator.SetFloat(_moveSpeed, _objectMovement.GetCurrentSpeed() / _objectMovement.GetMaxSpeed());
            _hatAnimator.SetFloat(_moveSpeed, _objectMovement.GetCurrentSpeed() / _objectMovement.GetMaxSpeed());

            _animator.SetFloat(_cursorX, InputSystem.CursorPosition.x - transform.position.x);
            _animator.SetFloat(_cursorY, InputSystem.CursorPosition.y - transform.position.y);
            _hatAnimator.SetFloat(_cursorX, InputSystem.CursorPosition.x - transform.position.x);
            _hatAnimator.SetFloat(_cursorY, InputSystem.CursorPosition.y - transform.position.y);

            _objectMovement.SetDirection(new Vector2(InputSystem.Movement.x, InputSystem.Movement.y));
            UpdateDebug();

            if (_currentDashCooldown > 0)
            {
                _currentDashCooldown -= Time.deltaTime;
            }
        } 
    }

    private void Dash() 
    {
        if (!IsDashUpgraded) return;

        if (_currentDashCooldown > 0) return;

        StartCoroutine(DashCoroutine(_objectMovement.GetMaxSpeed()));
    }

    private IEnumerator DashCoroutine(float oldSpeed)
    {
        _objectMovement.SetMaxSpeed(_dashSpeed);
        yield return new WaitForSeconds(_dashDuration);
        _objectMovement.SetMaxSpeed(oldSpeed);
        _currentDashCooldown = _dashCooldown;
    }

    private void UpdateDebug()
    {
        _direction = InputSystem.Movement;
    }

}
