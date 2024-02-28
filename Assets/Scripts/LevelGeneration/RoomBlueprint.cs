using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBlueprint
{
    public class Corridor
    {
        public static int _lastUsedId = 0;
        private int _id;
        private RoomBlueprint _destination;
        private List<Vector2> _gridPoints;

        public Corridor(RoomBlueprint destination, List<Vector2> gridPoints, int id)
        {
            _id = id;
            _destination = destination;
            _gridPoints = new List<Vector2>(gridPoints);
        }

        public RoomBlueprint GetDestination()
        {
            return _destination;
        }

        public int GetId() 
        { 
            return _id; 
        }

        public List<Vector2> GetGridPoints()
        {
            return _gridPoints;
        }

        private SimpleGrid GetAllignmentGrid()
        {
            return _destination._allignmentGrid;
        }

        public List<Vector2> GridPointToTiles(Vector2 gridPoint)
        {
            return RoomBlueprint.GridPointToTiles(gridPoint, GetAllignmentGrid());
        }

        public List<Vector2> GetTiles()
        {
            var bigPoints = GetGridPoints();

            var points = new List<Vector2>();

            foreach (var point in bigPoints)
            {
                points.AddRange(RoomBlueprint.GridPointToTiles(point, GetAllignmentGrid()));
            }
            return points;
        }

        /*
        public List<Vector2> GetTiles()
        {
            return _tiles;
        }
        */
    }
    
    private static int _lastUsedId = 0;
    private readonly int _id;
    private List<Corridor> _corridors = new List<Corridor>();
    private Rectangle _bounds;// { get; private set; }
    private SimpleGrid _allignmentGrid;

    public RoomBlueprint(Vector2 center, float width, float height, SimpleGrid allignmentGrid)
    {
        _id = _lastUsedId;
        _lastUsedId += 1;
        _bounds = new Rectangle(center, width, height);
        _allignmentGrid = allignmentGrid;
    }

    public int GetId()
    {
        return _id;
    }

    public List<Corridor> GetCorridors()
    {
        return _corridors;
    }

    public float GetWidth() => _bounds.GetWidth();

    public float GetHeight() => _bounds.GetHeight();

    public Vector2 GetCenter() => _bounds.GetCenter();

    public Vector2 GetUpperLeft() => _bounds.GetUpperLeft();

    public Vector2 GetUpperRight() => _bounds.GetUpperRight();

    public Vector2 GetLowerLeft() => _bounds.GetLowerLeft();

    public Vector2 GetLowerRight() => _bounds.GetLowerRight();

    public List<Vector2> GetAllGridPoints() => _bounds.GetAllGridPoints(_allignmentGrid);

    private static List<Vector2> GridPointToTiles(Vector2 gridPoint, SimpleGrid allignmentGrid)
    {
        var points = new List<Vector2>();

        var ofs = new Vector2(.5f, .5f);
        if (allignmentGrid.GetXGap() == 1)
            ofs = Vector2.zero;

        var bounds = new Rectangle(ofs, allignmentGrid.GetXGap(), allignmentGrid.GetXGap());
        var localPoints = bounds.GetAllGridPoints(new SimpleGrid(1));
        foreach (var localPoint in localPoints)
        {
            points.Add(localPoint + gridPoint - ofs);
        }

        return points;
    }

    public List<Vector2> GetAllTiles()
    {
        var bigPoints = _bounds.GetAllGridPoints(_allignmentGrid);

        var points = new List<Vector2>();

        foreach (var point in bigPoints)
        {
            points.AddRange(GridPointToTiles(point, _allignmentGrid));
        }
        return points;
    }

    public List<Vector2> GetPerimeterGridPoints() => _bounds.GetPerimeterGridPoints(_allignmentGrid);

    public bool IsConnectedTo(RoomBlueprint room)
    {
        foreach (var corridor in _corridors)
        {
            if (corridor.GetDestination() == room)
                return true;
        }

        return false;
    }

    public static void ConnectRoomsWithCorridor(RoomBlueprint roomA, RoomBlueprint roomB, List<Vector2> connectingGridPoints)
    {
        if (roomA.IsConnectedTo(roomB) || roomB.IsConnectedTo(roomA))
            throw new System.Exception($"Room with id {roomA._id} is already connected to room with id {roomB._id}");

        var corridorId = Corridor._lastUsedId;
        Corridor._lastUsedId += 1;

        var tilesForA = new List<Vector2>(connectingGridPoints);

        var tilesForB = new List<Vector2>(connectingGridPoints);
        tilesForB.Reverse();
        roomA._corridors.Add(new Corridor(roomB, tilesForA, corridorId));
        roomB._corridors.Add(new Corridor(roomA, tilesForB, corridorId));
    }


}
