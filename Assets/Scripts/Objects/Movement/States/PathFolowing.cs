using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFolowing : ObjectMovementMainState
{

    private List<Vector3> _path;
    public PathFolowing(ObjectMovementMainState objectMovement) : base(objectMovement)
    {
    }

    public PathFolowing(Rigidbody2D rigidbody, float accelerationTime, float decelerationTime, float changeDirectionTime, float maxSpeed, List<Vector3> path) : base(rigidbody, accelerationTime, decelerationTime, changeDirectionTime, maxSpeed)
    {
        _path = path;
    }

    public override ObjectMovementMainState Update(Vector2 direction, float deltaTime)
    {

        if (_path.Count > 0)
        {
            direction = GetDirectionTo((Vector2)_path[0]);
            Vector2 velocity = direction * GetMaxSpeed();
            GetRigidbody().AddForce((velocity - GetRigidbody().velocity) * GetRigidbody().mass / deltaTime);
            if (Vector2.Distance(GetRigidbody().transform.position, (Vector2)_path[0]) < 0.1f)
            {
                _path.RemoveAt(0);
            }
        } else
        {
            return new Idle(this);
        }

       

        return this;
    }

    public Vector2 GetDirectionTo(Vector2 point)
    {
        Vector2 direction = point - (Vector2)GetRigidbody().transform.position;
        return direction.normalized;
    }
}
