using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private int _initialWidth = 50;
    [SerializeField]
    private int _initialHeight = 50;
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
    private bool _debugDrawAllTiles = false;
    [SerializeField]
    private bool _debugDrawPerimeterTiles = false;
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
        SliceLeaves(_root, _iterCount);
        GenerateConnectionsForTree(_root);
    }

    public void SliceLeaves(RoomNode rootNode, int iterCount)
    {
        for (int i = 0; i < iterCount; i++)
        {
            List<RoomNode> leaves = rootNode.GetLeaves();
            foreach (var leaf in leaves)
            {
                var direction = leaf.GetHeight() > leaf.GetWidth() ? RoomNode.SliceDirection.horizontal : RoomNode.SliceDirection.vertical;
                leaf.Slice(direction, Random.Range(1, 4), Random.Range(1, 4));
            }   
        }

        if (_cutOffSomeLeafs )
        {
            foreach (var leaf in rootNode.GetLeaves())
            {
                if (leaf.GetSister() != null)
                    leaf.RemoveFromParent();
            }
        }
    }

    (RoomNode roomA, RoomNode roomB) FindTwoClosestRooms(List<RoomNode> listA, List<RoomNode> listB)
    {
        if (listA.Count == 0)
            throw new System.Exception("List A is empty. Both lists need to have elements inside");

        if (listB.Count == 0)
            throw new System.Exception("List B is empty. Both lists need to have elements inside");

        RoomNode firstRoom = null;
        RoomNode secondRoom = null;
        float minDistance = float.MaxValue;
        foreach (var nodeA in listA)
        {
            foreach (var nodeB in listB)
            {
                var distBetween = Vector2.Distance(nodeA.GetCenter(), nodeB.GetCenter());
                if (distBetween < minDistance)
                {
                    minDistance = distBetween;
                    firstRoom = nodeA;
                    secondRoom = nodeB;
                }
            }
        }
        return (firstRoom, secondRoom);
    }
    
    public (Vector2 tileA, Vector2 tileB) FindConnectingTiles(RoomNode roomA, RoomNode roomB)
    {
        Vector2 tileA, tileB;
        bool success;
        (tileA, tileB) = FindConnectingTiles(roomA, roomB, true, out success);
        if (!success)
            (tileA, tileB) = FindConnectingTiles(roomA, roomB, false, out success);

        if (!success)
            throw new System.Exception("Critical error! Could not find connecting tiles! Something went very wrong!!!");

        return (tileA, tileB);
    }

    private (Vector2 tileA, Vector2 tileB) FindConnectingTiles(RoomNode roomA, RoomNode roomB, bool excludeDiagonals, out bool success)
    {
        success = false;

        Vector2 firstRoomTile = Vector2.zero;
        Vector2 secondRoomTile = Vector2.zero;

        float minDistance = float.MaxValue;
        float minDistToCenters = float.MaxValue;

        var roomATiles = roomA.GetPerimeterGridPoints();
        var roomBTiles = roomB.GetPerimeterGridPoints();
        foreach (var tileA in roomATiles)
        {
            foreach (var tileB in roomBTiles)
            {
                bool isDiagonal = !(tileA.x == tileB.x || tileA.y == tileB.y);

                if (isDiagonal && excludeDiagonals)
                    continue;

                var distBetweenTiles = Vector2.Distance(tileA, tileB);
                if (distBetweenTiles <= minDistance)
                {
                    minDistance = distBetweenTiles;

                    Vector2 toTileB = tileB - tileA;
                    Vector2 tileMiddlePoint = tileA + (0.5f * toTileB.magnitude) * toTileB.normalized;
                    var distToCenters = Vector2.Distance(tileA, roomB.GetCenter()) + Vector2.Distance(tileB, roomA.GetCenter());

                    if (distToCenters < minDistToCenters)
                    {
                        minDistToCenters = distToCenters;
                        firstRoomTile = tileA;
                        secondRoomTile = tileB;
                        success = true;
                    }
                }
            }
        }

        return (firstRoomTile, secondRoomTile);
    }
    
    void GenerateConnectionsForTree(RoomNode rootNode)
    {
        //For each node in tree
        var allNodes = rootNode.GetAllNodes();
        foreach (var node in allNodes)
        {
            //Try to connect to sister node
            var sister = node.GetSister();
            if (sister != null)
            {
                //Find two closest rooms
                var currentLeaves = node.GetLeaves();
                var sisterLeaves = sister.GetLeaves();
                RoomNode firstRoom = null;
                RoomNode secondRoom = null;
                (firstRoom, secondRoom) = FindTwoClosestRooms(currentLeaves, sisterLeaves);
                if (firstRoom == null || secondRoom == null)
                    throw new System.Exception("Critical error! Could not find connecting tiles! Something went very wrong!!!");

                //Connect rooms if they are not connected
                if (!firstRoom.IsConnectedTo(secondRoom))
                {
                    //Find connecting tiles (to connect the rooms)
                    Vector2 firstRoomTile = Vector2.zero;
                    Vector2 secondRoomTile = Vector2.zero;
                    (firstRoomTile, secondRoomTile) = FindConnectingTiles(firstRoom, secondRoom);

                    //Create a connection
                    RoomNode.ConnectNodes(firstRoom, firstRoomTile, secondRoom, secondRoomTile);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_root != null)
            DrawRoomNodeTree(_root, _debugDrawRoomEdges, _debugDrawRoomCenter, _debugDrawAllTiles, _debugDrawPerimeterTiles, _debugDrawRoomConnections, _debugDrawRoomCorridors);
    }

    private void DrawRoomNodeTree(RoomNode rootNode, bool drawNodeEdges, bool drawNodeCenters, bool drawAllRoomTiles, bool drawRoomPerimeterTiles, bool drawCenterConnections, bool drawConnections) 
    {
        var rooms = rootNode.GetLeaves();
        foreach (var room in rooms)
        {
            if (drawAllRoomTiles)
            {
                var tiles = room.GetAllGridPoints();
                foreach (var tile in tiles)
                {
                    DebugDraw.DrawCell(tile, 1, Color.cyan);
                }
            }
            else if (drawRoomPerimeterTiles)
            {
                var tiles = room.GetPerimeterGridPoints();
                foreach (var tile in tiles)
                {
                    DebugDraw.DrawCell(tile, 1, Color.cyan);
                }
            }

            if (drawNodeEdges)
            {
                DebugDraw.DrawRectangle(
                    room.GetUpperLeft(),
                    room.GetUpperRight(),
                    room.GetLowerRight(),
                    room.GetLowerLeft(),
                    Color.blue);
            }

            if (drawNodeCenters)
            {
                DebugDraw.DrawCross(room.GetCenter(), 1, Color.green);
            }

            if (drawCenterConnections || drawConnections)
            {
                foreach (var connection in room.GetRoomConnections())
                {
                    if (drawCenterConnections)
                        DebugDraw.DrawLine(connection._start.GetCenter(), connection._end.GetCenter(), Color.red);

                    if (drawConnections)
                        DebugDraw.DrawLine(connection._startTilePos, connection._endTilePos, Color.magenta);
                }
            }
        }
        
    }
}
