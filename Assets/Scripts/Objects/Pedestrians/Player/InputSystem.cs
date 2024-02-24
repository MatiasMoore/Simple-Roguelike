using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public static Vector2 Movement;
    
    public static Vector2 CursorPosition;

    public static InputSystem Instance { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _moveAction;

    private InputAction _cursorPosition;

    private InputAction _cursorClick;

    public UnityEvent<Vector2> CursorClickEvent;

    [SerializeField]
    private bool _isDebugOn;

    public void Init()

    {
        if (Instance != null) return;
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];

        _cursorPosition = _playerInput.actions["CursorPosition"];

        _cursorClick = _playerInput.actions["CursorClick"];

        _cursorClick.performed += (var) => OnCursorClick();

        Instance = this;
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        CursorPosition = _cursorPosition.ReadValue<Vector2>();   
        
        if (_isDebugOn)
            Debug.Log($"Cursor position: {CursorPosition}");
        

    }

    public void OnCursorClick()
    {
        if (_isDebugOn)
            Debug.Log("Cursor click");
        CursorClickEvent?.Invoke(CursorPosition);
    }   

}
