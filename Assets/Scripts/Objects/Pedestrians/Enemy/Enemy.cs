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
    private float _preferedDistance = 5f;
    [SerializeField]
    private float _distanceToChase = 5f;

    [Header("Configuration")]
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private ObjectMovement _objectMovement;

    private CombatStatePrimitive _currentState;

    public enum CombatState
    {
        Idle, Follow
    }

    private Dictionary<CombatState, CombatStatePrimitive> _states = new Dictionary<CombatState, CombatStatePrimitive>();

    public void SwitchToState(CombatState stateEnum)
    {
        if (_states.TryGetValue(stateEnum, out CombatStatePrimitive state))
        {
            _currentState.Stop();
            _currentState = state;
            _currentState.Start();
        }
        else
            throw new System.Exception("No state could be found for the given state enum!");
    }

    private void Start()
    {
        _objectMovement.Init();
        _objectMovement.SetWalkType(ObjectMovement.WalkType.ByPoint);
        InitStates();
    }

    private void InitStates()
    {
        if (_currentState != null)
            _currentState.Stop();

        _states.Clear();
        _states.Add(CombatState.Idle, new IdleState(this, _player, this.transform, _distanceToChase));
        _states.Add(CombatState.Follow, new FollowPlayerState(this, _player, this.transform, _objectMovement, _preferedDistance));

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
        _currentState.DebugDrawGizmos();
    }
}
