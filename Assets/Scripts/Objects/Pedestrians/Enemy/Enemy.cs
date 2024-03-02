using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        Vector3 playerPos = _player.transform.position;
        playerPos.z = 0;
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(playerPos, path);
        
        Vector3[] pathvec = path.corners;
        List<Vector2> pathVec2d = new List<Vector2>();
        foreach (var vec in pathvec)
        {
            pathVec2d.Add((Vector2) vec);
        }
        _objectMovement.WalkTo(pathVec2d);
    }

    private void GoToPoint(Vector2 point)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(point, path);

        Vector3[] pathvec = path.corners;
        List<Vector2> pathVec2d = new List<Vector2>();
        foreach (var vec in pathvec)
        {
            pathVec2d.Add((Vector2)vec);
        }
        _objectMovement.WalkTo(pathVec2d);
    }
}
