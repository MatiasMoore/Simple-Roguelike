using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public static Vector2 Movement;

    public static InputSystem Instance { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _moveAction;

    public void Init()

    {
        if (Instance != null) return;
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];

        Instance = this;
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
    }

}
