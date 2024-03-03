using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MovementAIStatePrimitive 
{
    protected MovementAIStateManager _stateManager;

    public MovementAIStatePrimitive(MovementAIStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    abstract public void Start();

    abstract public void Stop();

    abstract public void Update();

    abstract public void DebugDrawGizmos();

    protected bool CanSeeObject(Transform self, Transform obj)
    {
        var hit = Physics2D.Raycast(self.position, obj.position - self.position, Vector2.Distance(self.position, obj.position), LayerMask.GetMask("Wall"));
        return hit.collider == null;
    }
}
