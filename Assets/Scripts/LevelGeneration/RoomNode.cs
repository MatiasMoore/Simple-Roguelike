using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Rectangle _bounds { get; private set; }

    private RoomNode _parent;
    private List<RoomNode> _children = new List<RoomNode>();
    private List<RoomNode> _connections = new List<RoomNode>();

    public RoomNode(Vector2 center, float width, float height)
    {
        _parent = null;
        _bounds = new Rectangle(center, width, height);
    }

    private RoomNode(RoomNode parent, Vector2 center, float width, float height)
    {
        parent._children.Add(this);
        _parent = parent;
        _bounds = new Rectangle(center, width, height);
    }

    public bool IsConnectedTo(RoomNode node)
    {
        return _connections.Contains(node);
    }

    public List<RoomNode> GetRoomConnections()
    {
        return _connections;
    }

    public static void ConnectNodes(RoomNode nodeA, RoomNode nodeB)
    {
        if (nodeA.IsConnectedTo(nodeB) || nodeB.IsConnectedTo(nodeA))
            throw new System.Exception($"Node at {nodeA._bounds.GetCenter()} is already connected to node at {nodeB._bounds.GetCenter()}");

        nodeA._connections.Add(nodeB);
        nodeB._connections.Add(nodeA);
    }

    public List<RoomNode> GetChildren()
    {
        return new List<RoomNode>(_children);
    }

    public int GetChildrenCount()
    {
        return _children.Count;
    }

    public RoomNode GetParent()
    {
        return _parent;
    }

    public enum SliceDirection
    {
        vertical, horizontal
    }

    public void Slice(SliceDirection direction, int firstRatio, int secondRatio, SimpleGrid allignmentGrid, float minSize)
    {
        if (firstRatio <= 0 || secondRatio <= 0)
            throw new System.Exception($"Slicing node at {_bounds.GetCenter()} failed. First and second ration must be positive numbers");

        if (_children.Count > 0)
            throw new System.Exception($"Slicing node at {_bounds.GetCenter()} failed. Node is already sliced");

        if (direction == SliceDirection.vertical)
        {
            float firstHalf = allignmentGrid.SnapToGrid(firstRatio * (_bounds.GetWidth() / (firstRatio + secondRatio)));
            float secondHalf = _bounds.GetWidth() - firstHalf;

            if (Mathf.Min(firstHalf, secondHalf) < minSize)
            {
                Debug.LogWarning($"Stopped slicing node at {_bounds.GetCenter()} because of size limit");
                return;
            }

            new RoomNode(this, _bounds.GetLowerLeft() + new Vector2(firstHalf / 2, _bounds.GetHeight() / 2), firstHalf, _bounds.GetHeight());
            new RoomNode(this, _bounds.GetLowerRight() + new Vector2(-secondHalf / 2, _bounds.GetHeight() / 2), secondHalf, _bounds.GetHeight());
        }
        else
        {
            float firstHalf = allignmentGrid.SnapToGrid(firstRatio * (_bounds.GetHeight() / (firstRatio + secondRatio)));
            float secondHalf = _bounds.GetHeight() - firstHalf;

            if (Mathf.Min(firstHalf, secondHalf) < minSize)
            {
                Debug.LogWarning($"Stopped slicing node at {_bounds.GetCenter()} because of size limit");
                return;
            }

            new RoomNode(this, _bounds.GetUpperLeft() + new Vector2(_bounds.GetWidth() / 2, -firstHalf / 2), _bounds.GetWidth(), firstHalf);
            new RoomNode(this, _bounds.GetLowerLeft() + new Vector2(_bounds.GetWidth() / 2, secondHalf / 2), _bounds.GetWidth(), secondHalf);
        }
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
