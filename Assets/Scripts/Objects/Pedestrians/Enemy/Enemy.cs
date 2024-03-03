using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ObjectMovement))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private ObjectMovement _objectMovement;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _objectMovement.Init();
        _objectMovement.SetWalkType(ObjectMovement.WalkType.ByPoint);
        InputSystem.Instance.CursorClickEvent += GoToPoint;
    }

    private void Update()
    {
        
    }

    [ContextMenu("Go to player")]
    private void GoToPlayer()
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(_player.transform.position, path);
        _objectMovement.WalkTo(path.corners.ToList());
    }

    private void GoToPoint(Vector2 point)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(point, path);
        _objectMovement.WalkTo(path.corners.ToList());
    }
}
