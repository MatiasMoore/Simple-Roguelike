using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private float _initialWidth = 50;
    [SerializeField]
    private float _initialHeight = 50;
    [SerializeField]
    private Vector2 _initialCenter = new Vector2(0, 0);
    [SerializeField]
    private int _iterCount = 4;

    [SerializeField]
    private bool _debugDrawLeaves = true;

    private RoomNode _root;

    private void OnEnable()
    {
        GenerateNewLevel();
    }

    public void GenerateNewLevel()
    {
        _root = new RoomNode(_initialCenter, _initialWidth, _initialHeight);
        SliceLeaves(_iterCount);
    }

    public void SliceLeaves(int iterCount)
    {
        for (int i = 0; i < iterCount; i++)
        {
            List<RoomNode> leaves = _root.GetLeaves();
            foreach (var leaf in leaves)
            {
                var direction = Random.Range(0, 2) == 0 ? RoomNode.SliceDirection.horizontal : RoomNode.SliceDirection.vertical;
                leaf.Slice(direction, Random.Range(1, 4), Random.Range(1, 4));
            }   
        }
    }

    void Update()
    {
        if (_debugDrawLeaves)
        {
            var leaves = _root.GetLeaves();
            foreach (var leave in leaves)
            {
                leave.DebugDraw(Color.blue);
            }
        }


    }
}
