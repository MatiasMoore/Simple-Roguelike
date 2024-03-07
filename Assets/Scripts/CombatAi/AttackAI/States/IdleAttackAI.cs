using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAttackAI : AttackAIStatePrimitive
{
    private Transform _self;

    private Transform _player;

    private float _attackDistance;

    public IdleAttackAI(AttackAIStateManager stateManager, Transform player, Transform self, float attackDistance) : base(stateManager)
    {
        _self = self;
        _player = player;
        _attackDistance = attackDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _attackDistance, Color.red);
    }

    public override void Start()
    {

    }

    public override void Stop()
    {

    }

    public override void Update()
    {
        if (_player == null)
        {
            return;
        }

        if (Vector2.Distance(_self.position, _player.position) < _attackDistance && CanSeeObject(_self, _player))
        {
            _stateManager.SwitchToState(AttackAIStateManager.AttackState.Attack);

        }
        
    }
}
