using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static AttackAIStateManager;

public class MovementAIStateManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _reload = false;

    [SerializeField]
    private float _lostAggroDistance = 10f;

    [SerializeField]
    private float _startAggroDistance = 8f;

    [SerializeField]
    private float _preferredDistance = 3f; 

    [Header("Configuration")]

    private static Transform _player;

    [SerializeField]
    private CollisionListener _aggroCollider;

    [SerializeField]
    private ObjectMovement _objectMovement;

    private MovementAIStatePrimitive _currentState;

    public enum MovementState
    {
        Sleep, Calm, Follow, Idle
    }

    private MovementState _currentStateEnum;

    private Dictionary<MovementState, MovementAIStatePrimitive> _states = new Dictionary<MovementState, MovementAIStatePrimitive>();

    public void SwitchToState(MovementState stateEnum)
    {
        if (_states.TryGetValue(stateEnum, out MovementAIStatePrimitive state))
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

    private void OnEnable()
    {
        if (_player == null)
        {
            _aggroCollider.gameObject.SetActive(true);
        }

        _aggroCollider.OnTriggerEnter += OnAggroTriggerEnter;

        _objectMovement.Init();
        _objectMovement.SetWalkType(ObjectMovement.WalkType.ByPoint);
        InitStates();
        _currentState = _states[MovementState.Sleep];

        if (_player == null)
        {
            _aggroCollider.gameObject.SetActive(true);
            _currentState = _states[MovementState.Sleep];
        }
        else
        {
            _currentState = _states[MovementState.Calm];
        }
    }

    private void InitStates()
    {
        if (_currentState != null)
        {
            _currentState.Stop();
        }
            
        _states.Clear();

        _states.Add(MovementState.Sleep, new SleepMovementAI(this));
        _states.Add(MovementState.Idle, new IdleMovementAI(this, _player, transform, _preferredDistance));
        _states.Add(MovementState.Follow, new FollowPlayerMovementAI(this, _player, transform, _objectMovement, _lostAggroDistance, _preferredDistance));
        _states.Add(MovementState.Calm, new CalmMovementAI(this, _player, transform, _startAggroDistance));
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
            
        } else
        {
            // Draw all distances
            DebugDraw.DrawSphere(transform.position, _lostAggroDistance, Color.blue);
            DebugDraw.DrawSphere(transform.position, _startAggroDistance, Color.green);
            DebugDraw.DrawSphere(transform.position, _preferredDistance, Color.yellow);

        }
    }

    private void OnAggroTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player") && _currentState is SleepMovementAI)
        {
            _player = other.transform;
            InitStates();
            SwitchToState(MovementState.Calm);
            Debug.Log($"AImanager: {gameObject.name} noticed player! {_player.position}");
            _aggroCollider.gameObject.SetActive(false);
        }

    }

    private void OnDisable()
    {
        _aggroCollider.OnTriggerEnter -= OnAggroTriggerEnter;
    }
}
