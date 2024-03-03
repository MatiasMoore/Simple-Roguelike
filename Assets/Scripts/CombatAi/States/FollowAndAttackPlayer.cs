using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAndAttackPlayer : CombatStatePrimitive
{
    private Transform _player;

    private Transform _self;

    private ObjectMovement _movement;

    private Weapon _weapon;

    private float _maxDistance;

    private float _minDistance;

    public FollowAndAttackPlayer(CombatStateManager stateManager,
       Transform player, Transform self,
       ObjectMovement movement, Weapon weapon, float maxDistance, float minDistance) : base(stateManager)
    {
        _player = player;
        _self = self;
        _movement = movement;
        _weapon = weapon;
        _maxDistance = maxDistance;
        _minDistance = minDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _maxDistance, Color.red);
        DebugDraw.DrawSphere(_self.position, _minDistance, Color.red);
    }

    public override void Start()
    {
        Debug.Log("State: FollowAndAttackPlayer state started");
    }

    public override void Stop()
    {
        _weapon.RotateWeaponToPoint((Vector2)_self.position + Vector2.right);
        _movement.Stop();
    }

    public override void Update()
    {
        float distsanceToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distsanceToPlayer > _maxDistance)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.Follow);
            return;
        }
        
        if (distsanceToPlayer < _minDistance)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.IdleAndAttack);
            return;
        }
        
        _movement.GoToPointOnNavMesh(_player.position);
        _weapon.RotateWeaponToPoint(_player.position);
        _weapon.Enter();
    }
}

