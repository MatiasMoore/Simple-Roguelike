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

    [SerializeField]
    private CollisionListener _dangerousVision;

    [SerializeField]
    private float _evasionSphereRadius;

    [Range(0f, 360f), SerializeField]
    private float _minEvasionCorrelationAngle;

    [Range(0f, 360f), SerializeField]
    private float _maxEvasionCorrelationAngle;

    [Header("Configuration")]

    private static Transform _player;

    [SerializeField]
    private CollisionListener _aggroCollider;

    [SerializeField]
    private ObjectMovement _objectMovement;

    private MovementAIStatePrimitive _currentState;

    public enum MovementState
    {
        Sleep, Calm, Follow, Idle, Evading
    }

    private MovementState _currentStateEnum;
    public struct StateData
    {
        public Collider2D DangerousObject;
        public MovementState LastState;
    }

    public StateData stateData;


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

        _dangerousVision.OnTriggerEnter += EvasionCheck;
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
        _states.Add(MovementState.Evading, new EvadingMovementAI(this, transform, _objectMovement, _evasionSphereRadius, _minEvasionCorrelationAngle, _maxEvasionCorrelationAngle));
            
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

    private void OnDrawGizmosSelected()
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
            DebugDraw.DrawSphere(transform.position, _evasionSphereRadius, Color.magenta);
            DebugDraw.DrawSphere(transform.position, _dangerousVision.GetRadius(), Color.cyan);

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

    private void EvasionCheck(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            // if other damage enemy
            Projectile projectile = other.GetComponent<Projectile>();
            LayerMask whatIDamage = projectile.GetWhatIDamage();
            if((whatIDamage.value & 1 << gameObject.layer) == 0)
            {
                return;
            }
            Debug.Log("AI: EVADE!!!!!");
            if (_currentStateEnum != MovementState.Evading)
            {
                stateData.LastState = _currentStateEnum;
            }
            
            stateData.DangerousObject = other;
            SwitchToState(MovementState.Evading);
        }
    }
}
