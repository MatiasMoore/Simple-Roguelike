using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomNode
{
    public class RoomConnection
    {
        public RoomNode _start { get; private set; }
        public Vector2 _startTilePos { get; private set; }

        public RoomNode _end { get; private set; }
        public Vector2 _endTilePos { get; private set; }

        public RoomConnection(RoomNode start, Vector2 startTilePos, RoomNode end, Vector2 endTilePos)
        {
            _start = start;
            _startTilePos = startTilePos;

            _end = end;
            _endTilePos = endTilePos;
        }

        public bool Contains(RoomNode node)
        {
            return _start == node || _end == node;
        }
    }

    private Vector2 _center;
    private float _width;
    private float _height;

    private RoomNode _parent;
    private List<RoomNode> _children = new List<RoomNode>();
    private List<RoomConnection> _connections = new List<RoomConnection>();

    public RoomNode(Vector2 center, float width, float height)
    {
        _parent = null;
        _center = center;
        _width = width;
        _height = height;
    }

    private RoomNode(RoomNode parent, Vector2 center, float width, float height)
    {
        parent._children.Add(this);
        _parent = parent;
        _center = center;
        _width = width;
        _height = height;
    }

    public bool IsConnectedTo(RoomNode node) 
    {
        foreach (var connection in _connections)
        {
            if (connection.Contains(node))
                return true;
        }
        return false;
    }

    public List<RoomConnection> GetRoomConnections()
    {
        return _connections;
    }

    public static void ConnectNodes(RoomNode nodeA, Vector2 nodeATilePos, RoomNode nodeB, Vector2 nodeBTilePos)
    {
        if (nodeA.IsConnectedTo(nodeB) || nodeB.IsConnectedTo(nodeA)) 
            throw new System.Exception($"Node at {nodeA._center} is already connected to node at {nodeB._center}");

        nodeA._connections.Add(new RoomConnection(nodeA, nodeATilePos, nodeB, nodeBTilePos));
        nodeB._connections.Add(new RoomConnection(nodeB, nodeBTilePos, nodeA, nodeATilePos));
    }

    public List<RoomNode> GetChildren()
    {
        return new List<RoomNode>(_children);
    }

    public int GetChildrenCount()
    {
        return _children.Count;
    }

    public List<Vector2> GetAllGridPoints()
    {
        var points = new List<Vector2>();

        const float margin = 0.0f;
        float maxX = _center.x + _width / 2 - margin;
        float minX = _center.x - _width / 2 + margin;
        float maxY = _center.y + _height / 2 - margin;
        float minY = _center.y - _height / 2 + margin;

        const float step = 0.3f;

        var start = GetLowerLeft();

        for (int i = 0; i <= _width / step; i++)
        {
            for (int j = 0; j <= _height / step; j++)
            {
                Vector2 newPoint = start + new Vector2(i * step, j * step);
                newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);

                bool isWithinBounds = newPoint.x < maxX && newPoint.x > minX
                    && newPoint.y < maxY && newPoint.y > minY;

                if (!points.Contains(newPoint) && isWithinBounds)
                    points.Add(newPoint);
            }
        }
        return points;
    }

    //This entire function is not very well written
    //but it will be fine for now
    //(the performance is not bad there's just way too much repetition)
    public List<Vector2> GetPerimeterGridPoints()
    {
        var points = new List<Vector2>();

        const float margin = 0.0f;
        float maxX = _center.x + _width / 2 - margin;
        float minX = _center.x - _width / 2 + margin;
        float maxY = _center.y + _height / 2 - margin;
        float minY = _center.y - _height / 2 + margin;

        float step = 0.3f;

        //Get top row
        for (int i = 0; i <= _width / step; i++)
        {
            Vector2 newPoint = GetUpperLeft() + new Vector2(i * step, 0);
            newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);

            if (newPoint.y >= maxY)
            {
                newPoint.y -= 1;
                newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);
            }

            bool isWithinBounds = newPoint.x < maxX && newPoint.x > minX
                    && newPoint.y < maxY && newPoint.y > minY;

            if (!points.Contains(newPoint) && isWithinBounds)
                points.Add(newPoint);
        }

        //Get bottom row
        for (int i = 0; i <= _width / step; i++)
        {
            Vector2 newPoint = GetLowerLeft() + new Vector2(step, step) + new Vector2(i * step, 0);
            newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);

            if (newPoint.y <= minY)
            {
                newPoint.y += 1;
                newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);
            }

            bool isWithinBounds = newPoint.x < maxX && newPoint.x > minX
                    && newPoint.y < maxY && newPoint.y > minY;

            if (!points.Contains(newPoint) && isWithinBounds) 
                points.Add(newPoint);
        }

        //Get left collumn
        for (int i = 0; i <= _height / step; i++)
        {
            Vector2 newPoint = GetLowerLeft() + new Vector2(step, step) + new Vector2(0, i * step);
            newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);

            if (newPoint.x <= minX)
            {
                newPoint.x += 1;
                newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);
            }

            bool isWithinBounds = newPoint.x < maxX && newPoint.x > minX
                    && newPoint.y < maxY && newPoint.y > minY;

            if (!points.Contains(newPoint) && isWithinBounds) 
                points.Add(newPoint);
        }

        //Get right collumn
        for (int i = 0; i <= _height / step; i++)
        {
            Vector2 newPoint = GetLowerRight() + new Vector2(-step, step) + new Vector2(0, i * step);
            newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);

            if (newPoint.x >= maxX)
            {
                newPoint.x -= 1;
                newPoint = new Vector2((int)newPoint.x, (int)newPoint.y);
            }

            bool isWithinBounds = newPoint.x < maxX && newPoint.x > minX
                    && newPoint.y < maxY && newPoint.y > minY;

            if (!points.Contains(newPoint) && isWithinBounds) 
                points.Add(newPoint);
        }

        return points;
    }

    public RoomNode GetParent()
    {
        return _parent;
    }

    public float GetWidth()
    {
        return _width;
    }

    public float GetHeight()
    {
        return _height;
    }

    public Vector2 GetCenter()
    {
        return _center;
    }

    public Vector2 GetUpperLeft()
    {
        return _center + new Vector2(-_width / 2, _height / 2);
    }

    public Vector2 GetUpperRight()
    {
        return _center + new Vector2(_width / 2, _height / 2);
    }

    public Vector2 GetLowerLeft()
    {
        return _center + new Vector2(-_width / 2, -_height / 2);
    }

    public Vector2 GetLowerRight()
    {
        return _center + new Vector2(_width / 2, -_height / 2);
    }

    public enum SliceDirection
    {
        vertical, horizontal
    }

    public void Slice(SliceDirection direction, int firstRatio, int secondRatio)
    {
        if (firstRatio <= 0 || secondRatio <= 0)
            throw new System.Exception($"Slicing node at {_center} failed. First and second ration must be positive numbers");

        if (_children.Count > 0)
            throw new System.Exception($"Slicing node at {_center} failed. Node is already sliced");

        if (direction == SliceDirection.vertical)
        {
            float firstHalf = (int)(firstRatio * (_width / (firstRatio + secondRatio)));
            float secondHalf = _width - firstHalf;

            if (Mathf.Min(firstHalf, secondHalf) < 4)
            {
                Debug.LogWarning($"Stopped slicing node at {_center} because of size limit");
                return;
            }

            new RoomNode(this, GetLowerLeft() + new Vector2(firstHalf / 2, _height / 2), firstHalf, _height);
            new RoomNode(this, GetLowerRight() + new Vector2(-secondHalf / 2, _height / 2), secondHalf, _height);
        }
        else
        {
            float firstHalf = (int)(firstRatio * (_height / (firstRatio + secondRatio)));
            float secondHalf = _height - firstHalf;

            if (Mathf.Min(firstHalf, secondHalf) < 4)
            {
                Debug.LogWarning($"Stopped slicing node at {_center} because of size limit");
                return;
            }

            new RoomNode(this, GetUpperLeft() + new Vector2(_width / 2, -firstHalf / 2), _width, firstHalf);
            new RoomNode(this, GetLowerLeft() + new Vector2(_width / 2, secondHalf / 2), _width, secondHalf);
        }
    }

    public void DebugDrawTree()
    {
        DebugDrawTree(Color.blue);
    }

    public void DebugDrawTree(Color color)
    {
        this.DebugDraw(color);
        foreach (var child in _children)
        {
            child.DebugDrawTree(color);
        }
    }

    public void DebugDraw()
    {
        DebugDraw(Color.blue);
    }

    public void DebugDraw(Color color) 
    {
        Debug.DrawLine(GetUpperLeft(), GetUpperRight(), color);
        Debug.DrawLine(GetLowerRight(), GetLowerLeft(), color);
        Debug.DrawLine(GetLowerLeft(), GetUpperLeft(), color);
        Debug.DrawLine(GetLowerRight(), GetUpperRight(), color);
    }

    public List<RoomNode> GetLeaves()
    {
        var leaves = new List<RoomNode>();
        GetLeavesInternal(leaves);
        return leaves;
    }

    public List<RoomNode> GetAllNodes()
    {
        var nodes = new List<RoomNode>();
        GetAllNodesInternal(nodes);
        return nodes;
    }

    private void GetAllNodesInternal(List<RoomNode> nodes)
    {
        nodes.Add(this);
        foreach (var child in _children) 
        {
            child.GetAllNodesInternal(nodes);
        }
    }

    private void GetLeavesInternal(List<RoomNode> leaves)
    {
        foreach (var child in _children)
        {
            child.GetLeavesInternal(leaves);
        }

        if (_children.Count == 0)
            leaves.Add(this);
    }

    public RoomNode GetSister()
    {
        RoomNode sister = null;
        if (_parent != null)
        {
            var sisters = _parent._children;
            foreach (var s in sisters)
            {
                if (s != this)
                    sister = s;
            }
        }
        return sister;
    }

    public void RemoveFromParent()
    {
        _parent._children.Remove(this);
    }
}
