using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerMovementAI : MovementAIStatePrimitive
{
    private Transform _player;
    private Transform _self;
    private ObjectMovement _movement;
    private float _maxDistance;
    private float _minDistance;
    private Vector2 _lastSeenPlayerPosition;

    public FollowPlayerMovementAI(MovementAIStateManager stateManager, 
        Transform player, Transform self, 
        ObjectMovement movement,float maxDistance, float minDistance) : base(stateManager)
    {
        _player = player;
        _self = self;
        _movement = movement;
        _minDistance = minDistance;
        _maxDistance = maxDistance;
    }

    public override void DebugDrawGizmos()
    {
        DebugDraw.DrawSphere(_self.position, _minDistance, Color.red);
        DebugDraw.DrawSphere(_self.position, _maxDistance, Color.green);
        DebugDraw.DrawCross(_lastSeenPlayerPosition, 2f, Color.magenta);
    }

    public override void Start()
    {
        if (CanSeeObject(_self, _player))
        {
            _lastSeenPlayerPosition = _player.position;
        } else
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Calm);
        }
        
        Debug.Log("State: Follow state started");
    }

    public override void Stop()
    {
        _movement.Stop();
    }

    public override void Update()
    {
        if (_player == null)
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Calm);
            return;
        }

        var distToPlayer = Vector2.Distance(_player.position, _self.position);

        if (distToPlayer < _minDistance)
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Idle);
            return;
        }      

        if (distToPlayer > _maxDistance)
        {
            MoveToLastSeenPlayerPosition();
            return;
        }

        if (!CanSeeObject(_self, _player))
        {
            MoveToLastSeenPlayerPosition();
        }
        else
        {
            _lastSeenPlayerPosition = _player.position;
            _movement.GoToPointOnNavMesh(_player.position);
        }

    }

    private void MoveToLastSeenPlayerPosition()
    {
        NavMeshHit hit = new();
        if (NavMesh.SamplePosition(_lastSeenPlayerPosition, out hit, 99999, NavMesh.AllAreas))
        {
            _movement.GoToPointOnNavMesh(hit.position);

            if (Vector2.Distance(hit.position, _self.position) < 0.01f)
            {
                _stateManager.SwitchToState(MovementAIStateManager.MovementState.Calm);
                return;
            }

        } else
        {
            _stateManager.SwitchToState(MovementAIStateManager.MovementState.Calm);
            return;
        }
       
        
    }
}
