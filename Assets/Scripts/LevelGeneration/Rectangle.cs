using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rectangle 
{
    private readonly float _width, _height;
    private readonly Vector2 _center;

    public Rectangle(Vector2 center, float width, float height)
    {
        _center = center;
        _width = width;
        _height = height;
    }

    public Rectangle ScaleBy(float scale)
    {
        return new Rectangle(scale * _center, scale * _width, scale * _height);
    }

    private float GetMinX()
    {
        return _center.x - _width / 2;
    }

    private float GetMaxX()
    {
        return _center.x + _width / 2;
    }

    private float GetMinY()
    {
        return _center.y - _height / 2;
    }

    private float GetMaxY()
    {
        return _center.y + _height / 2;
    }

    public bool DoesIntersect(Rectangle rect)
    {
        return this.GetMinX() < rect.GetMaxX()
            && this.GetMaxX() > rect.GetMinX()
            && this.GetMinY() < rect.GetMaxY()
            && this.GetMaxY() > rect.GetMinY();
    }

    private List<float> GetGridCoordsMinMax(float min, float max, SimpleGrid allignmentGrid)
    {
        var possibleValues = new List<float>();

        var start = allignmentGrid.SnapToGridOnX(min);
        if (start < min)
            start += allignmentGrid.GetXGap();

        var currentX = start;
        while (currentX <= max)
        {
            possibleValues.Add(currentX);
            currentX += allignmentGrid.GetXGap();
        }
        return possibleValues;
    }

    private (List<float> possibleXValues, List<float> possibleYValues) GetPossibleXYValuesInGrid(SimpleGrid allignmentGrid)
    {
        float tileSize = allignmentGrid.GetXGap();
        float maxX = _center.x + _width / 2 - tileSize / 2;
        float minX = _center.x - _width / 2 + tileSize / 2;
        float maxY = _center.y + _height / 2 - tileSize / 2;
        float minY = _center.y - _height / 2 + tileSize / 2;
        var possibleXValues = GetGridCoordsMinMax(minX, maxX, allignmentGrid);
        var possibleYValues = GetGridCoordsMinMax(minY, maxY, allignmentGrid);

        return (possibleXValues, possibleYValues);
    }

    public List<Vector2> GetAllGridPoints(SimpleGrid allignmentGrid)
    {
        var points = new List<Vector2>();

        List<float> possibleXValues, possibleYValues;
        (possibleXValues, possibleYValues) = GetPossibleXYValuesInGrid(allignmentGrid);

        if (possibleXValues.Count > 0 && possibleYValues.Count > 0) 
        {
            foreach (var y in possibleYValues)
            {
                foreach(var x in possibleXValues)
                {
                    points.Add(new Vector2(x, y));
                }
            }
        }

        return points;
    }

    public List<Vector2> GetPerimeterGridPoints(SimpleGrid allignmentGrid)
    {
        var points = new List<Vector2>();

        List<float> possibleXValues, possibleYValues;
        (possibleXValues, possibleYValues) = GetPossibleXYValuesInGrid(allignmentGrid);

        //Top row and bottom row
        var topY = possibleYValues.Last();
        var bottomY = possibleYValues.First();
        foreach (var x in possibleXValues)
        {
            points.Add(new Vector2(x, topY));
            points.Add(new Vector2(x, bottomY));
        }

        //Left collumn and right collumn
        var leftX = possibleXValues.First();
        var rightX = possibleXValues.Last();
        foreach (var y in possibleYValues)
        {
            points.Add(new Vector2(leftX, y));
            points.Add(new Vector2(rightX, y));
        }

        return points;
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
}
