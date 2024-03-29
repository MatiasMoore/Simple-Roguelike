using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMovementAI : MovementAIStatePrimitive
{
    private Transform _player;
    private Transform _self;
    private float _aggroDistance;

    public IdleMovementAI(MovementAIStateManager stateManager, Transform player, Transform self, float aggroDistance) : base(stateManager)
    {
        _aggroDistance = aggroDistance;
        _player = player;
        _self = self;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _aggroDistance, Color.green);
    }

    public override void Start()
    {
        Debug.Log("State: Idle state started");
    }

    public override void Stop()
    {
    }

    public override void Update()
    {
        if (_player == null)
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Calm);
            return;
        }

        var distToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distToPlayer > _aggroDistance)
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Follow);
            return;
        }

        if (!CanSeeObject(_self, _player))
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Follow);
            return;
        }
    }
}
