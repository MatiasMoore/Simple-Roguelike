using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CombatStatePrimitive 
{
    protected CombatStateManager _stateManager;

    public CombatStatePrimitive(CombatStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    abstract public void Start();

    abstract public void Stop();

    abstract public void Update();

    abstract public void DebugDrawGizmos();
}
