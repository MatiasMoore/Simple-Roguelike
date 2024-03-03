using System;
using UnityEngine;

[Serializable]
public class SimpleGrid
{
    [SerializeField]
    private float _gap;

    public SimpleGrid(float gap)
    {
        _gap = gap;
    }

    public float GetGap()
    {
        return _gap;
    }

    public Vector2 SnapToGrid(Vector2 pos)
    {
        var gridAlligned = Vector2.zero;

        gridAlligned.x = SnapToGrid(pos.x, _gap);
        gridAlligned.y = SnapToGrid(pos.y, _gap);

        return gridAlligned;
    }

    public float SnapToGrid(float coord)
    {
        return SnapToGrid(coord, _gap);
    }

    private float SnapToGrid(float coord, float gap)
    {
        float gridAlligned;

        float coordAbs = Mathf.Abs(coord);

        var remainder = coordAbs / gap - Mathf.Floor(coordAbs / gap);
        if (remainder < gap / 2)
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
