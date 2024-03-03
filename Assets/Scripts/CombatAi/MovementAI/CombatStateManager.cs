using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectMovement))]
public class CombatStateManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _reload = false;

    [SerializeField]
    private float _lostAggroDistance = 10f;

    [SerializeField]
    private float _startAggroDistance = 8f;

    [SerializeField]
    private float _attackDistance = 6f;

    [SerializeField]
    private float _preferredDistance = 3f; 

    [Header("Configuration")]
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private ObjectMovement _objectMovement;

    [SerializeField]
    private Weapon _weapon;

    private CombatStatePrimitive _currentState;

    public enum CombatState
    {
        Idle, Follow, FollowAndAttack, IdleAndAttack
    }

    private CombatState _currentStateEnum;

    private Dictionary<CombatState, CombatStatePrimitive> _states = new Dictionary<CombatState, CombatStatePrimitive>();

    public void SwitchToState(CombatState stateEnum)
    {
        if (_states.TryGetValue(stateEnum, out CombatStatePrimitive state))
        {
            _currentStateEnum = stateEnum;
            _currentState.Stop();
            _currentState = state;
            _currentState.Start();    
        }
        else
        {
            throw new System.Exception("No state could be found for the given state enum!");
        }
            
    }

    private void Start()
    {
        _objectMovement.Init();
        _objectMovement.SetWalkType(ObjectMovement.WalkType.ByPoint);
        _weapon.Init();
        InitStates();
    }

    private void InitStates()
    {
        if (_currentState != null)
        {
            _currentState.Stop();
        }
            
        _states.Clear();
        _states.Add(CombatState.Idle, new IdleState(this, _player, transform, _startAggroDistance));
        _states.Add(CombatState.Follow, new FollowPlayerState(this, _player, transform, _objectMovement, _lostAggroDistance, _attackDistance));
        _states.Add(CombatState.FollowAndAttack, new FollowAndAttackPlayer(this, _player, transform, _objectMovement, _weapon, _attackDistance, _preferredDistance));
        _states.Add(CombatState.IdleAndAttack, new IdleAndAttack(this, _player, transform, _weapon, _attackDistance));

        _currentState = _states[CombatState.Idle];
    }

    private void Update()
    {
        _currentState.Update();
    }

    private void OnValidate()
    {
        if (_reload)
        {
            _reload = false;
            InitStates();
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            _currentState.DebugDrawGizmos();
        } else
        {
            // Draw all distances
            DebugDraw.DrawSphere(transform.position, _lostAggroDistance, Color.blue);
            DebugDraw.DrawSphere(transform.position, _startAggroDistance, Color.green);
            DebugDraw.DrawSphere(transform.position, _attackDistance, Color.red);
            DebugDraw.DrawSphere(transform.position, _preferredDistance, Color.yellow);

        }
    }
}
