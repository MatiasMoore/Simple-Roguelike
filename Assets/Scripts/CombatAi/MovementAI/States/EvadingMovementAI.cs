using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingMovementAI : MovementAIStatePrimitive
{
    private Transform _self;

    private Collider2D _dangerousObject;

    private ObjectMovement _movement;

    private float _radius;

    private bool _isDirectionChosen = false;
    private Vector2 _evasionVector;
    private Vector2 _intersection;

    public EvadingMovementAI(MovementAIStateManager stateManager, Transform self, Collider2D dangerousObject, ObjectMovement objectMovement, float sphereRadius) : base(stateManager)
    {
        _self = self;
        _dangerousObject = dangerousObject;
        _movement = objectMovement;
        _radius = sphereRadius;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _radius, Color.magenta);
        DebugDraw.DrawCross(_intersection, 1, Color.green);
    }

    public override void Start()
    {
  
    }

    public override void Stop()
    {
 
    }

    public override void Update()
    {
        if (_dangerousObject == null)
        {
            _stateManager.SwitchToState(_stateManager.stateData.LastState);
            return;
        }

        // get velocity vector
        Vector2 dangerousVelocity = _dangerousObject.GetComponent<ObjectMovement>().GetMaxSpeed() * _dangerousObject.transform.right;

        Vector2 dangerousPosition = (Vector2)_dangerousObject.transform.position;
        Vector2 velocityNormalized = dangerousVelocity.normalized;
        Vector2 selfPosition = _self.position;
        
        if (IntersectRaySphere(dangerousPosition, velocityNormalized, selfPosition, _radius, out _intersection))
        {
            if (!_isDirectionChosen || _evasionVector == Vector2.zero)
            {
                _evasionVector = Vector2.Perpendicular(dangerousVelocity.normalized);
                if (Vector2.Distance(_evasionVector, _intersection) < Vector2.Distance(-_evasionVector, _intersection))
                {
                    _evasionVector = -_evasionVector;
                }

                Debug.Log($"AI: Evasion vector: {_evasionVector}");
                _isDirectionChosen = true;
            }
                
            if (dangerousVelocity.magnitude != 0)
            {
                _movement.GoToPointOnNavMesh((Vector2)_self.position + _evasionVector);
            }                
        } else
        {
            _stateManager.SwitchToState(_stateManager.stateData.LastState);
            return;
        }
    }

    // Intersects ray r = p + td, |d| = 1, with sphere s and, if intersecting, 
    // returns t value of intersection and intersection point q 
    private bool IntersectRaySphere(Vector2 p, Vector2 d, Vector2 s, float r, out Vector2 q)
    {
        q = Vector2.zero;
        Vector2 m = p - s;
        float b = Vector2.Dot(m, d);
        float c = Vector2.Dot(m, m) - r * r;

        // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
        if (c > 0.0f && b > 0.0f) return false;
        float discr = b * b - c;

        // A negative discriminant corresponds to ray missing sphere 
        if (discr < 0.0f) return false;

        // Ray now found to intersect sphere, compute smallest t value of intersection
        float t = -b - Mathf.Sqrt(discr);

        // If t is negative, ray started inside sphere so clamp t to zero 
        if (t < 0.0f) t = 0.0f;
        q = p + t * d;

        return true;
    }
}