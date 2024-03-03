using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerState : CombatStatePrimitive
{
    private Transform _player;
    private Transform _self;
    private ObjectMovement _movement;
    private float _preferredDistance;

    public FollowPlayerState(CombatStateManager stateManager, 
        Transform player, Transform self, 
        ObjectMovement movement, float preferredDistance) : base(stateManager)
    {
        _player = player;
        _self = self;
        _movement = movement;
        _preferredDistance = preferredDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _preferredDistance, Color.green);
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
        _movement.GoToPointOnNavMesh(_self.position);
    }

    public override void Update()
    {
        var distToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distToPlayer > _preferredDistance)
        {
            _movement.GoToPointOnNavMesh(_player.position);
        }
        else
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.Idle);
        }
    }
}
