using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    private List<RoomBlueprint> _rooms;
    private Rectangle _bounds;

    public Level(List<RoomBlueprint> rooms, Vector2 totalCenter, float totalWidth, float totalHeight)
    {
        _rooms = new List<RoomBlueprint>(rooms);
        _bounds = new Rectangle(totalCenter, totalWidth, totalHeight);
    }

    public List<RoomBlueprint> GetRooms()
    {
        return _rooms;
    }

    public Vector2 GetCenter() => _bounds.GetCenter();

    public float GetWidth() => _bounds.GetWidth();

    public float GetHeight() => _bounds.GetHeight();
}
