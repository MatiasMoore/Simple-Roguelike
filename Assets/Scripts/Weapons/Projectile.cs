using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _accelerationTime = 0.1f;
    [SerializeField]
    private float _decelerationTime = 0.1f;
    [SerializeField]
    private float _changeDirectionTime = 5f;
    [SerializeField]
    private float _maxSpeed = 5f;

    [SerializeField]
    private float _aliveTime = 5f;

    private float _timer;

    private ObjectMovement _objectMovement;

    public void Start()
    {
        _objectMovement = new Idle(GetComponent<Rigidbody2D>(), _accelerationTime, _decelerationTime, _changeDirectionTime, _maxSpeed);
    }
    
    public void Update()
    {  
        _timer += Time.deltaTime;
        _objectMovement = _objectMovement.Update(transform.right, _timer);
        if (_timer >= _aliveTime)
        {
            Destroy(gameObject);
        }
    }

}
