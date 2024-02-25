using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGenerator
{
    private RoomNode _root;
    private int _iterCount;
    private bool _cutoffSomeLeaves;
    private Vector2 _centerOffset;
    private int _width, _height;

    private int _seed;
    private System.Random _random;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private Task<RoomNode> _mainTask;

    private string _currentStatus;
    private bool _isStatusUpdated = false;

    public LevelGenerator()
    {
        SetStatusString("Idle");
    }

    public bool IsGenerationInProgress()
    {
        return _mainTask != null && !_mainTask.IsCompleted;
    }

    private void SetStatusString(string newStatus)
    {
        _currentStatus = newStatus;
        _isStatusUpdated = true;
    }

    public string GetStatusString()
    {
        _isStatusUpdated = false;
        return _currentStatus;
    }

    public bool IsStatusUpdated()
    {
        return _isStatusUpdated;
    }

    public Task<RoomNode> GenerateNewLevel(Vector2 centerOffset, int width, int height, int iterCount, bool cutoffSomeLeaves, int seed)
    {
        _centerOffset = centerOffset;
        _width = width;
        _height = height;
        _root = new RoomNode(_centerOffset, _width, _height);
        _iterCount = iterCount;
        _cutoffSomeLeaves = cutoffSomeLeaves;
        SetNewSeed(seed);

        if (_mainTask != null && !_mainTask.IsCompleted)
            AbortTasks();

        _mainTask = GenerateNewLevelTask(_seed, _tokenSource);
        return _mainTask;
    }

    public void AbortTasks()
    {
        SetStatusString("Interrupted and idle");
        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
    }

    private Task<RoomNode> GenerateNewLevelTask(int newSeed, CancellationTokenSource tokenSource)
    {
        SetStatusString("Initialising generation");
        var t = new Task<RoomNode>(() =>
        {
            SetNewSeed(newSeed);

            var sliceTask = SliceLeavesTask(_root, _iterCount, tokenSource);
            sliceTask.Wait();

            if (tokenSource.Token.IsCancellationRequested)
                tokenSource.Token.ThrowIfCancellationRequested();

            if (_cutoffSomeLeaves)
            {
                foreach (var leaf in _root.GetLeaves())
                {
                    if (leaf.GetSister() != null)
                        leaf.RemoveFromParent();
                }
            }

            var connectionsTask = GenerateConnectionsForTreeTask(_root, tokenSource);
            connectionsTask.Wait();

            if (tokenSource.Token.IsCancellationRequested)
                tokenSource.Token.ThrowIfCancellationRequested();

            SetStatusString("Finished generating!");

            return _root;
        }, tokenSource.Token);
        t.Start();
        return t;
    }

    public static void DebugDrawLevel(RoomNode rootNode, bool drawNodeEdges, bool drawNodeCenters, bool drawAllRoomTiles, bool drawRoomPerimeterTiles, bool drawCenterConnections, bool drawConnections)
    {
        if (!drawNodeEdges && !drawNodeCenters && !drawAllRoomTiles && !drawRoomPerimeterTiles && !drawCenterConnections && !drawConnections)
            return;

        if (rootNode == null)
            throw new System.Exception("Critical error! The level's root node is null! Something went very wrong!!!");

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

    private Task SliceLeavesTask(RoomNode rootNode, int iterCount, CancellationTokenSource tokenSource)
    {
        var t = new Task(() =>
        {
            for (int i = 0; i < iterCount; i++)
            {
                List<RoomNode> leaves = rootNode.GetLeaves();
                foreach (var leaf in leaves)
                {
                    var direction = leaf.GetHeight() > leaf.GetWidth() ? RoomNode.SliceDirection.horizontal : RoomNode.SliceDirection.vertical;
                    leaf.Slice(direction, _random.Next(1, 4), _random.Next(1, 4));

                    if (tokenSource.Token.IsCancellationRequested)
                        tokenSource.Token.ThrowIfCancellationRequested();
                }

                SetStatusString($"Finished slicing iteration {i+1} out of {iterCount}");
            }
        }, tokenSource.Token);
        t.Start();  
        return t;
    }

    private Task GenerateConnectionsForTreeTask(RoomNode rootNode, CancellationTokenSource tokenSource)
    {
        var t = new Task(() =>
        {
            //For each node in tree
            var allNodes = rootNode.GetAllNodes();
            for (int i = 0; i < allNodes.Count; i++)
            {
                var node = allNodes[i];

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

                SetStatusString($"Finished connecting node {i+1} out of {allNodes.Count}");

                if (tokenSource.Token.IsCancellationRequested)
                    tokenSource.Token.ThrowIfCancellationRequested();
            }
        }, tokenSource.Token);
        t.Start();
        return t;
    }

    private (RoomNode roomA, RoomNode roomB) FindTwoClosestRooms(List<RoomNode> listA, List<RoomNode> listB)
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
    
    private (Vector2 tileA, Vector2 tileB) FindConnectingTiles(RoomNode roomA, RoomNode roomB)
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

    private void SetNewSeed(int newSeed)
    {
        _seed = newSeed;
        _random = new System.Random(_seed);
    }
}
