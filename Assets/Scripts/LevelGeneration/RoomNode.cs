using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomNode
{
    private Vector2 _center;
    private float _width;
    private float _height;

    private RoomNode _parent;
    private List<RoomNode> _children = new List<RoomNode>();

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

    public List<RoomNode> GetChildren()
    {
        return _children;
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
            float firstHalf = firstRatio * (_width / (firstRatio + secondRatio));
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
            float firstHalf = firstRatio * (_height / (firstRatio + secondRatio));
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
