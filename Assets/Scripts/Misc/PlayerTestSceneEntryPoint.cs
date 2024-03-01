using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestSceneEntryPoint : MonoBehaviour
{
    [SerializeField]
    private InputSystem _inputSystem;

    [SerializeField]
    private PlayerMovementController _movementController;

    [SerializeField]
    private PlayerWeaponController _weaponController;

    private void Start()
    {
        if (_inputSystem != null)
        {
            _inputSystem.Init();
            if (_movementController != null)
            {
                _movementController.Init(_inputSystem);
            } else
            {
                Debug.LogWarning("PlayerMovementController is not assigned");
            }

            if (_weaponController != null)
            {
                _weaponController.Init(_inputSystem);
            } else
            {
                Debug.LogWarning("PlayerWeaponController is not assigned");
            }
        } else
        {
            Debug.LogWarning("InputSystem is not assigned");
        }   
             
    }
}
