using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectMovement))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private ObjectMovement _objectMovement;

    private void Start()
    {
        _objectMovement.Init();
        _objectMovement.SetWalkType(ObjectMovement.WalkType.ByPoint);
        InputSystem.Instance.CursorClickEvent += GoToPoint;
    }

    private List<Vector3> _debugPath = new List<Vector3>();

    private void GoToPoint(Vector2 point)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath((Vector2)transform.position, point, NavMesh.AllAreas, path))
        {
            _objectMovement.WalkTo(path.corners.ToList());
            _debugPath = path.corners.ToList();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var pos in _debugPath)
        {
            DebugDraw.DrawCross(pos, 0.2f, Color.green);
        }
    }
}
