using System.Collections.Generic;
using System.Linq;
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
    SimpleGrid _allignmentGrid;

    private int _seed;
    private System.Random _random;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private Task<List<RoomBlueprint>> _mainTask;

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

    public Task<List<RoomBlueprint>> GenerateNewLevel(Vector2 centerOffset, int width, int height, SimpleGrid allignmentGrid, int iterCount, bool cutoffSomeLeaves, int seed)
    {
        _centerOffset = centerOffset;
        _width = width;
        _height = height;
        _allignmentGrid = allignmentGrid;
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

    private Task<List<RoomBlueprint>> GenerateNewLevelTask(int newSeed, CancellationTokenSource tokenSource)
    {
        SetStatusString("Initialising generation");
        var t = new Task<List<RoomBlueprint>>(() =>
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

            var roomsTask = GenerateConnectedRooms(_root, tokenSource);
            roomsTask.Wait();

            if (tokenSource.Token.IsCancellationRequested)
                tokenSource.Token.ThrowIfCancellationRequested();

            SetStatusString("Finished generating!");

            return roomsTask.Result;
        }, tokenSource.Token);
        t.Start();
        return t;
    }
    
    public void DebugDrawLevel(List<RoomBlueprint> rooms, bool drawNodeEdges, bool drawNodeCenters, bool drawAllGridPoints, bool drawPerimeterGridPoints, bool drawAllRoomTiles, bool drawCenterConnections, bool drawConnections)
    {
        if (rooms == null)
            throw new System.Exception("Critical error! The level's root node is null! Something went very wrong!!!");

        foreach (var room in rooms)
        {
            if (drawAllRoomTiles)
            {
                var tiles = room.GetAllTiles();
                foreach (var tile in tiles)
                {
                    DebugDraw.DrawCell(tile, 1, Color.cyan);
                }
            }

            if (drawAllGridPoints)
            {
                var gridPoints = room.GetAllGridPoints();
                foreach (var gridPoint in gridPoints)
                {
                    DebugDraw.DrawCell(gridPoint, 1, Color.red);
                }
            }
            else if (drawPerimeterGridPoints)
            {
                var gridPoints = room.GetPerimeterGridPoints();
                foreach (var gridPoint in gridPoints)
                {
                    DebugDraw.DrawCell(gridPoint, 1, Color.red);
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
                foreach (var connection in room.GetCorridors())
                {
                    if (drawCenterConnections)
                        DebugDraw.DrawLine(room.GetCenter(), connection.GetDestination().GetCenter(), Color.red);

                    if (drawConnections)
                        DebugDraw.DrawLine(connection.GetGridPoints().First(), connection.GetGridPoints().Last(), Color.magenta);
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
                    var direction = leaf._bounds.GetHeight() > leaf._bounds.GetWidth() ? RoomNode.SliceDirection.horizontal : RoomNode.SliceDirection.vertical;
                    leaf.Slice(direction, _random.Next(1, 4), _random.Next(1, 4), _allignmentGrid, 4 * _allignmentGrid.GetGap());

                    if (tokenSource.Token.IsCancellationRequested)
                        tokenSource.Token.ThrowIfCancellationRequested();
                }

                SetStatusString($"Finished slicing iteration {i+1} out of {iterCount}");
            }
        }, tokenSource.Token);
        t.Start();  
        return t;
    }

    static private List<Vector2> CreatePath(Vector2 start, Vector2 finish, float step)
    {
        var path = new List<Vector2>();

        Vector2 toSecondTile = finish - start;
        Vector2 toSecondTileDir = toSecondTile.normalized;
        for (int j = 0; j <= toSecondTile.magnitude / step; j++)
        {
            Vector2 newPoint = start + j * step * toSecondTileDir;
            path.Add(newPoint);
        }

        return path;
    }

    private Task<List<RoomBlueprint>> GenerateConnectedRooms(RoomNode rootNode, CancellationTokenSource tokenSource)
    {
        var t = new Task<List<RoomBlueprint>>(() =>
        {
            //Create the list
            var roomBlueprints = new Dictionary<RoomNode, RoomBlueprint>();

            //Create blueprints for all leaves(only they will become rooms)
            var allLeaves = rootNode.GetLeaves();
            foreach (var leaf in allLeaves)
            {
                var newBlueprint = new RoomBlueprint(
                leaf._bounds.GetCenter(),
                leaf._bounds.GetWidth(),
                leaf._bounds.GetHeight(),
                _allignmentGrid);
                roomBlueprints.Add(leaf, newBlueprint);
            }

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
                        Debug.LogError("Critical error! Could not find connecting tiles! Something went very wrong!!!");

                    //Connect rooms if they are not connected
                    if (!firstRoom.IsConnectedTo(secondRoom))
                    {
                        //Create a connection
                        RoomNode.ConnectNodes(firstRoom, secondRoom);

                        RoomBlueprint firstBlueprint, secondBlueprint;
                        bool foundFirst = roomBlueprints.TryGetValue(firstRoom, out firstBlueprint);
                        bool foundSecond = roomBlueprints.TryGetValue(secondRoom, out secondBlueprint);
                        if (!foundFirst || !foundSecond)
                            Debug.LogError("Critical error! Couldn't find a blueprint for the node! Something went very wrong!");

                        //Find connecting tiles (to connect the rooms)
                        Vector2 firstRoomTile = Vector2.zero;
                        Vector2 secondRoomTile = Vector2.zero;
                        (firstRoomTile, secondRoomTile) = FindConnectingTiles(firstBlueprint, secondBlueprint);

                        //Create a tile path
                        List<Vector2> connectingGridPoints = new List<Vector2>();

                        bool isDiagonal = !(firstRoomTile.x == secondRoomTile.x || firstRoomTile.y == secondRoomTile.y);

                        if (!isDiagonal)
                        {
                            connectingGridPoints = CreatePath(firstRoomTile, secondRoomTile, _allignmentGrid.GetGap());
                        }
                        else
                        {
                            var middlePoint = new Vector2(firstRoomTile.x, secondRoomTile.y);
                            if (_random.Next(0, 2) == 1)
                                middlePoint = new Vector2(secondRoomTile.x, firstRoomTile.y);

                            connectingGridPoints = CreatePath(firstRoomTile, middlePoint, _allignmentGrid.GetGap());
                            connectingGridPoints.Remove(middlePoint);
                            connectingGridPoints.AddRange(CreatePath(middlePoint, secondRoomTile, _allignmentGrid.GetGap()));
                        }

                        

                        //Create a corridor
                        RoomBlueprint.ConnectRoomsWithCorridor(firstBlueprint, secondBlueprint, connectingGridPoints);
                    }
                }

                SetStatusString($"Finished connecting node {i+1} out of {allNodes.Count}");

                if (tokenSource.Token.IsCancellationRequested)
                    tokenSource.Token.ThrowIfCancellationRequested();

            }

            
            return new List<RoomBlueprint>(roomBlueprints.Values.ToList());
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
                var distBetween = Vector2.Distance(nodeA._bounds.GetCenter(), nodeB._bounds.GetCenter());
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
    
    private (Vector2 tileA, Vector2 tileB) FindConnectingTiles(RoomBlueprint roomA, RoomBlueprint roomB)
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

    private (Vector2 tileA, Vector2 tileB) FindConnectingTiles(RoomBlueprint roomA, RoomBlueprint roomB, bool excludeDiagonals, out bool success)
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
