using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestSceneEntryPoint : MonoBehaviour
{
    [SerializeField]
    private InputSystem _inputSystem;

    [SerializeField]
    private PlayerMovement _player;

    private void Start()
    {
        if (_inputSystem != null)
        {
            _inputSystem.Init();
            if (_player != null)
            {
                _player.Init(_inputSystem);
            } else
            {
                Debug.LogWarning("PlayerMovement is not assigned");
            }
        } else
        {
            Debug.LogWarning("InputSystem is not assigned");
        }   
             
    }
}
