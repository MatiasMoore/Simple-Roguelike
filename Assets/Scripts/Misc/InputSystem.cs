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

    public static bool IsCursorPressed;

    public static InputSystem Instance { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _moveAction;

    private InputAction _cursorPosition;

    private InputAction _cursorClick;

    private InputAction _cursorRelease;

    private InputAction _reloadAction;

    private InputAction _weaponNavigation;

    public UnityAction<Vector2> CursorClickEvent;

    public UnityAction<Vector2> CursorReleaseEvent;

    public UnityAction ReloadEvent;

    public UnityAction ChooseNextWeapon;

    public UnityAction ChoosePreviousWeapon;

    [SerializeField]
    private bool _isDebugOn;

    public void Init()

    {
        if (Instance != null) return;
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _cursorPosition = _playerInput.actions["CursorPosition"];
        _cursorClick = _playerInput.actions["CursorClick"];
        _cursorRelease = _playerInput.actions["CursorRelease"];
        _reloadAction = _playerInput.actions["Reload"];

        _cursorClick.performed += (var) => OnCursorClick();
        _cursorRelease.performed += (var) => OnCursorRelease();
        _reloadAction.performed += (var) => ReloadEvent?.Invoke();


        _weaponNavigation = _playerInput.actions["WeaponNavigation"];

        _weaponNavigation.performed += (var) => WeaponNavigation();

        Instance = this;
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        //translate cursor position to scene position
        CursorPosition = Camera.main.ScreenToWorldPoint(_cursorPosition.ReadValue<Vector2>());
        
        if (_isDebugOn)
            Debug.Log($"Cursor position: {CursorPosition}");
        

    }

    private void WeaponNavigation()
    {
        if (_weaponNavigation.ReadValue<float>() > 0)
        {
            Debug.Log("Next weapon");
            ChooseNextWeapon?.Invoke();
        } else
        {
            Debug.Log("Previous weapon");
            ChoosePreviousWeapon?.Invoke();
        }
            
    }

    private void OnCursorClick()
    {
        if (_isDebugOn)
            Debug.Log("Cursor click");
        CursorClickEvent?.Invoke(CursorPosition);
        IsCursorPressed = true;
    }

    private void OnCursorRelease()
    {
        if (_isDebugOn)
            Debug.Log("Cursor release");
        CursorReleaseEvent?.Invoke(CursorPosition);
        IsCursorPressed = false;
    }

}
