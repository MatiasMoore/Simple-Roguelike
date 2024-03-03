using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAndAttack : CombatStatePrimitive
{
    private Transform _player;

    private Transform _self;

    private Weapon _weapon;

    private float _maxDistance;

    public IdleAndAttack(CombatStateManager stateManager,
       Transform player, Transform self,
       Weapon weapon, float maxDistance) : base(stateManager)
    {
        _player = player;
        _self = self;
        _weapon = weapon;
        _maxDistance = maxDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _maxDistance, Color.red);
    }

    public override void Start()
    {
        Debug.Log("State: IdleAndAttack state started");
    }

    public override void Stop()
    {
    }

    public override void Update()
    {
        float distsanceToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distsanceToPlayer > _maxDistance)
        {
            _stateManager.SwitchToState(CombatStateManager.CombatState.FollowAndAttack);
            return;
        }

        _weapon.RotateWeaponToPoint(_player.position);
        _weapon.Enter();
    }
}

