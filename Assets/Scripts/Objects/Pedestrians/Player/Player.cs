using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pedestrian
{
    private Rigidbody2D _rigidbody;

    private InputSystem _inputSystem;
    
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
        } 
    }

    private void Move(Vector2 direction)
    {
        
        _rigidbody.velocity = direction * GetSpeed();
        Debug.Log($"direction {direction}, velocity {_rigidbody.velocity}");
    }

}
