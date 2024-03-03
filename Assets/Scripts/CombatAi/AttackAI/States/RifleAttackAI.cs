using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleAttackAI : AttackAIStatePrimitive
{
    private Transform _self;

    private Transform _player;

    private float _attackDistance;

    private Weapon _weapon;

    public RifleAttackAI(AttackAIStateManager stateManager, Transform player, Transform self, float attackDistance, Weapon weapon) : base(stateManager)
    {
        _self = self;
        _player = player;
        _attackDistance = attackDistance;
        _weapon = weapon;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _attackDistance, new Color(1.0f, 0.5f, 0.0f, 1.0f));
    }

    public override void Start()
    {
        _weapon.Init();
    }

    public override void Stop()
    {
        _weapon.Deinit();
    }

    public override void Update()
    {
        if (Vector2.Distance(_self.position, _player.position) > _attackDistance || !CanSeeObject(_self, _player))
        {
            _stateManager.SwitchToState(AttackAIStateManager.AttackState.Idle);
            return;
        }

        // Calculate direction where to rotate weapon to point where player will be in the future
        RifleData weaponData = ((Rifle)_weapon).Data;
        float bulletMaxSpeed = weaponData.ProjectileSpeed;
        float bulletTimeToTarget = Vector2.Distance(_self.position, _player.position) / bulletMaxSpeed;
        Vector2 futurePlayerPosition =(Vector2)_player.position + _player.GetComponent<Rigidbody2D>().velocity * bulletTimeToTarget;
        _weapon.RotateWeaponToPoint(futurePlayerPosition);
        _weapon.Enter(); 
    }
}
