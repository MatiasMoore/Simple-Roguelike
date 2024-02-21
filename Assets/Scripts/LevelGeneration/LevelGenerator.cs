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
    private bool _cutOffSomeLeafs = false;

    [SerializeField]
    private bool _debugDrawRoomEdges = true;
    [SerializeField]
    private bool _debugDrawRoomCenter = false;
    [SerializeField]
    private bool _debugDrawTiles = false;
    [SerializeField]
    private bool _debugDrawRoomConnections = false;
    [SerializeField]
    private bool _debugDrawRoomCorridors = true;


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
                var direction = leaf.GetHeight() > leaf.GetWidth() ? RoomNode.SliceDirection.horizontal : RoomNode.SliceDirection.vertical;
                leaf.Slice(direction, Random.Range(1, 4), Random.Range(1, 4));
            }   
        }

        if (_cutOffSomeLeafs )
        {
            foreach (var leaf in _root.GetLeaves())
            {
                if (leaf.GetSister() != null)
                    leaf.RemoveFromParent();
            }
        }
    }

    void DebugDrawCross(Vector2 pos, float length, Color color)
    {
        Debug.DrawLine(pos + new Vector2(-length, 0), pos + new Vector2(length, 0), color);
        Debug.DrawLine(pos + new Vector2(0, -length), pos + new Vector2(0, length), color);
    }

    void DebugDrawCell(Vector2 pos, float length, Color color)
    {
        float half = length / 2;
        Debug.DrawLine(pos + new Vector2(-half, half), pos + new Vector2(half, half), color);
        Debug.DrawLine(pos + new Vector2(-half, -half), pos + new Vector2(half, -half), color);

        Debug.DrawLine(pos + new Vector2(-half, -half), pos + new Vector2(-half, half), color);
        Debug.DrawLine(pos + new Vector2(half, -half), pos + new Vector2(half, half), color);
    }

    void Update()
    {
        var leaves = _root.GetLeaves();
        foreach (var leave in leaves)
        {
            if (_debugDrawRoomEdges)
                leave.DebugDraw(Color.blue);
            if (_debugDrawTiles)
            {
                var points = leave.GetAllGridPoints();
                foreach (var p1 in points)
                {
                    DebugDrawCell(p1, 1, Color.cyan);
                }
            }
        }

        var allNodes = _root.GetAllNodes();
        foreach (var node in allNodes)
        {
            var sister = node.GetSister();
            if (sister != null)
            {
                var currentLeaves = node.GetLeaves();
                var sisterLeaves = sister.GetLeaves();

                RoomNode firstRoom = null;
                RoomNode secondRoom = null;
                float minDistance = float.MaxValue;
                foreach (var p1 in currentLeaves)
                {
                    foreach (var p2 in sisterLeaves)
                    {
                        var d = Vector2.Distance(p1.GetCenter(), p2.GetCenter());
                        if (d < minDistance)
                        {
                            minDistance = d;
                            firstRoom = p1;
                            secondRoom = p2;
                        }
                    }
                }

                if (_debugDrawRoomConnections)
                    Debug.DrawLine(firstRoom.GetCenter(), secondRoom.GetCenter(), Color.red);

                if (_debugDrawRoomCenter)
                {
                    DebugDrawCross(firstRoom.GetCenter(), 2, Color.green);
                    DebugDrawCross(secondRoom.GetCenter(), 2, Color.green);
                }


                if (_debugDrawRoomCorridors)
                {
                    Vector2 firstRoomTile = Vector2.zero;
                    Vector2 secondRoomTile = Vector2.zero;

                    Vector2 toSecondRoomCenter = secondRoom.GetCenter() - firstRoom.GetCenter();
                    Vector2 roomMiddlePoint = firstRoom.GetCenter() + (0.5f * toSecondRoomCenter.magnitude) * toSecondRoomCenter.normalized;

                    minDistance = float.MaxValue;
                    float minDistanceToRoomMiddle = float.MaxValue;

                    foreach (var p1 in firstRoom.GetAllGridPoints())
                    {
                        foreach (var p2 in secondRoom.GetAllGridPoints())
                        {
                            var d = Vector2.Distance(p1, p2);
                            if (d <= minDistance)
                            {
                                minDistance = d;

                                Vector2 toSecondPoint = p2 - p1;
                                Vector2 tileMiddlePoint = p1 + (0.5f * toSecondPoint.magnitude) * toSecondPoint.normalized;
                                var distToRoomMiddle = Vector2.Distance(roomMiddlePoint, tileMiddlePoint);

                                if (distToRoomMiddle < minDistanceToRoomMiddle)
                                {
                                    minDistanceToRoomMiddle = distToRoomMiddle;
                                    firstRoomTile = p1;
                                    secondRoomTile = p2;
                                }
                            }
                        }
                    }

                    Debug.DrawLine(firstRoomTile, secondRoomTile, Color.magenta);
                }
            }
        }
    }
}
