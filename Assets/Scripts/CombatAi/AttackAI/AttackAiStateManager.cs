using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectMovement))]
public class AttackAIStateManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _reload = false;

    [SerializeField]
    private float _attackDistance = 10f;

    [Header("Configuration")]
    private Transform _player;

    [SerializeField]
    CollisionListener _aggroCollider;

    [SerializeField]
    private Weapon _weapon;

    private AttackAIStatePrimitive _currentState;

    public enum AttackState
    {
        Sleep, Idle, Attack
    }

    private AttackState _currentStateEnum;

    private Dictionary<AttackState, AttackAIStatePrimitive> _states = new Dictionary<AttackState, AttackAIStatePrimitive>();

    public void SwitchToState(AttackState stateEnum)
    {
        if (_states.TryGetValue(stateEnum, out AttackAIStatePrimitive state))
        {
            _currentStateEnum = stateEnum;
            _currentState.Stop();
            _currentState = state;
            _currentState.Start();
        }
        else
        {
            throw new System.Exception("No attack state could be found for the given state enum!");
        }

    }

    private void Start()
    {
        _aggroCollider.OnTriggerEnter += OnAggroTriggerEnter;

        InitStates();

        _currentState = _states[AttackState.Sleep];
    }

    private void InitStates()
    {
        if (_currentState != null)
        {
            _currentState.Stop();
        }

        _states.Clear();

        _states.Add(AttackState.Sleep, new SleepAttackAI(this));
        _states.Add(AttackState.Idle, new IdleAttackAI(this, _player, transform, _attackDistance));
        _states.Add(AttackState.Attack, new RifleAttackAI(this, _player, transform, _attackDistance, _weapon));
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
            if (_currentState != null)
            {
                _currentState.DebugDrawGizmos();
            }
            
        }
        else
        {
            // Draw all distances
            DebugDraw.DrawSphere(transform.position, _attackDistance, Color.red);
        }
    }

   private void OnAggroTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player") && _currentState is SleepAttackAI)
        {
            _player = other.transform;
            InitStates();
            SwitchToState(AttackState.Idle);
        }
    }
}
