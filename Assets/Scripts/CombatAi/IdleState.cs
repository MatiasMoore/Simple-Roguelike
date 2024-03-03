using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CombatStatePrimitive
{
    private Transform _player;
    private Transform _self;
    private float _distToChase;

    public IdleState(CombatStateManager stateManager, Transform player, Transform self, float distToChase) : base(stateManager)
    {
        _player = player;
        _self = self;
        _distToChase = distToChase;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _distToChase, Color.green);
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    public override void Update()
    {
        var distToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distToPlayer > _distToChase)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.Follow);
        }
        
    }
}
