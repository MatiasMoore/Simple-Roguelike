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
}
