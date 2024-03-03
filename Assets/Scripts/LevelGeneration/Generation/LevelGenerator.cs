using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGenerator
{
    [Serializable]
    public struct LevelGenerationData
    {
        [SerializeField]
        public int iterCount;
        [SerializeField]
        public bool cutoffSomeLeaves;
        [SerializeField]
        public Rectangle startingArea;
        [SerializeField]
        public SimpleGrid allignmentGrid;
        [SerializeField]
        public int seed;

        [SerializeField]
        public GameObject playerObj, enemyObj;
    }

    LevelGenerationData _data;

    private RoomNode _root;
    
    private System.Random _random;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private Task<Level> _mainTask;
    private Level.LevelData _levelData;

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

    public Task<Level> GenerateNewLevel(LevelGenerationData data)
    {
        _data = data;
        _root = new RoomNode(
            _data.startingArea.GetCenter(), 
            _data.startingArea.GetWidth(), 
            _data.startingArea.GetHeight());

        SetNewSeed(_data.seed);

        if (_mainTask != null && !_mainTask.IsCompleted)
            AbortTasks();

        _mainTask = GenerateNewLevelTask(_data.seed, _tokenSource);
        return _mainTask;
    }

    public void AbortTasks()
    {
        SetStatusString("Interrupted and idle");
        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
    }

    private Task<Level> GenerateNewLevelTask(int newSeed, CancellationTokenSource tokenSource)
    {
        SetStatusString("Initialising generation");
        var t = new Task<Level>(() =>
        {
            _levelData = new Level.LevelData();

            SetNewSeed(newSeed);

            var sliceTask = SliceLeavesTask(_root, _data.iterCount, tokenSource);
            sliceTask.Wait();

            if (tokenSource.Token.IsCancellationRequested)
                tokenSource.Token.ThrowIfCancellationRequested();

            if (_data.cutoffSomeLeaves)
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

            var furthestRoomsTask = FindTwoFurthestRooms(roomsTask.Result, tokenSource);
            furthestRoomsTask.Wait();

            RoomBlueprint startRoom, endRoom;
            (startRoom, endRoom) = furthestRoomsTask.Result;

            SetStatusString("Finished generating!");

            var spawnOnGrid = _data.allignmentGrid.SnapToGrid(startRoom.GetCenter());
            var endOnGrid = _data.allignmentGrid.SnapToGrid(endRoom.GetCenter());

            _levelData.rooms = roomsTask.Result;
            _levelData.bounds = _root._bounds;
            _levelData.spawnRoom = startRoom;
            _levelData.endRoom = endRoom;
            _levelData.playerSpawn = RoomBlueprint.GridPointToTiles(spawnOnGrid, _data.allignmentGrid).First();
            _levelData.levelEnd = RoomBlueprint.GridPointToTiles(endOnGrid, _data.allignmentGrid).First();

            return new Level(_levelData);
        }, tokenSource.Token);
        t.Start();
        return t;
    }

    public Task<(RoomBlueprint, RoomBlueprint)> FindTwoFurthestRooms(List<RoomBlueprint> rooms, CancellationTokenSource tokenSource)
    {
        var t = new Task<(RoomBlueprint, RoomBlueprint)>(() =>
        {
            float maxDist = float.MinValue;
            RoomBlueprint roomA = null;
            RoomBlueprint roomB = null;
            foreach(var room1 in rooms)
            {
                foreach(var room2 in rooms)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                        tokenSource.Token.ThrowIfCancellationRequested();

                    var currentDist = Vector2.Distance(room1.GetCenter(), room2.GetCenter());
                    if (currentDist > maxDist)
                    {
                        maxDist = currentDist;
                        roomA = room1;
                        roomB = room2;
                    }
                }
            }


            if (roomA == null || roomB == null)
                throw new System.Exception("Couldn't find two furthest rooms!");

            return (roomA, roomB);

        }, tokenSource.Token);
        t.Start();
        return t;
    }

    public void DebugDrawLevel(Level level, bool drawNodeEdges, bool drawNodeCenters, bool drawAllGridPoints, bool drawPerimeterGridPoints, bool drawAllRoomTiles, bool drawCenterConnections, bool drawConnections)
    {
        var rooms = level.GetRooms();

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
                    leaf.Slice(direction, _random.Next(1, 4), _random.Next(1, 4), _data.allignmentGrid, 4 * _data.allignmentGrid.GetGap());

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
                _data.allignmentGrid);
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
                            connectingGridPoints = CreatePath(firstRoomTile, secondRoomTile, _data.allignmentGrid.GetGap());
                        }
                        else
                        {
                            var middlePoint = new Vector2(firstRoomTile.x, secondRoomTile.y);
                            if (_random.Next(0, 2) == 1)
                                middlePoint = new Vector2(secondRoomTile.x, firstRoomTile.y);

                            connectingGridPoints = CreatePath(firstRoomTile, middlePoint, _data.allignmentGrid.GetGap());
                            connectingGridPoints.Remove(middlePoint);
                            connectingGridPoints.AddRange(CreatePath(middlePoint, secondRoomTile, _data.allignmentGrid.GetGap()));
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
        _data.seed = newSeed;
        _random = new System.Random(_data.seed);
    }
}
