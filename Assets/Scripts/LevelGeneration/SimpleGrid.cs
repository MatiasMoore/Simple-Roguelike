using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SimpleGrid
{
    private float _xGap, _yGap;
    /*
    public SimpleGrid(float xGap, float yGap)
    {
        _xGap = xGap;
        _yGap = yGap;
    }
    */

    public SimpleGrid(float gap)
    {
        _xGap = gap;
        _yGap = gap;
    }

    public float GetXGap()
    {
        return _xGap;
    }

    public float GetYGap()
    {
        return _yGap;
    }

    public Vector2 SnapToGrid(Vector2 pos)
    {
        var gridAlligned = Vector2.zero;

        gridAlligned.x = SnapToGrid(pos.x, _xGap);
        gridAlligned.y = SnapToGrid(pos.y, _yGap);

        return gridAlligned;
    }

    public float SnapToGridOnX(float coord)
    {
        return SnapToGrid(coord, _xGap);
    }

    public float SnapToGridOnY(float coord)
    {
        return SnapToGrid(coord, _yGap);
    }

    private float SnapToGrid(float coord, float gap)
    {
        float gridAlligned;

        float coordAbs = Mathf.Abs(coord);

        var remainder = coordAbs / gap - Mathf.Floor(coordAbs / gap);
        if (remainder < _xGap / 2)
        {
            gridAlligned = Mathf.Floor(coordAbs / gap) * gap;
        }
        else
        {
            gridAlligned = (Mathf.Floor(coordAbs / gap) * gap) + gap;
        }

        if (coord < 0)
            gridAlligned = -gridAlligned;

        return gridAlligned;
    }
}
