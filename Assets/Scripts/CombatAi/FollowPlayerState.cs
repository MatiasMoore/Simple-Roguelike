using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerState : CombatStatePrimitive
{
    private Transform _player;
    private Transform _self;
    private ObjectMovement _movement;
    private float _maxDistance;
    private float _minDistance;

    public FollowPlayerState(CombatStateManager stateManager, 
        Transform player, Transform self, 
        ObjectMovement movement,float maxDistance, float minDistance) : base(stateManager)
    {
        _player = player;
        _self = self;
        _movement = movement;
        _minDistance = minDistance;
        _maxDistance = maxDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _minDistance, Color.red);
        DebugDraw.DrawSphere(_self.position, _maxDistance, Color.green);
    }

    public override void Start()
    {
        Debug.Log("State: Follow state started");
    }

    public override void Stop()
    {
        _movement.Stop();
    }

    public override void Update()
    {
        var distToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distToPlayer < _minDistance)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.FollowAndAttack);
            return;
        }

        if (distToPlayer > _maxDistance)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.Idle);
            return;
        }

        _movement.GoToPointOnNavMesh(_player.position);
    }
}
