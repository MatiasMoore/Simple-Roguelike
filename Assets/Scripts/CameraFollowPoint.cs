using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    [Range(2, 100f), SerializeField]
    private float _sensitivity = 1;

    [SerializeField]
    private Transform _playerTransform;

    private Camera _camera;

    private Vector3 _targetPosition;

    private void Start()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (_playerTransform != null)
        {
            _targetPosition = InputSystem.CursorPosition;
            Vector3 followObjectPosition = (_targetPosition + (_sensitivity - 1) * _playerTransform.position) / _sensitivity;
            transform.position = followObjectPosition;
        }
        
    }
}
