using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public struct LevelData
    {
        public List<RoomBlueprint> rooms;
        public Rectangle bounds;
        public Vector2 playerSpawn;
        public Vector2 levelEnd;
    }

    private LevelData _data;

    public Level(LevelData data)
    {
        _data = data;
    }

    public List<RoomBlueprint> GetRooms()
    {
        return _data.rooms;
    }

    public Vector2 GetCenter() => _data.bounds.GetCenter();

    public float GetWidth() => _data.bounds.GetWidth();

    public float GetHeight() => _data.bounds.GetHeight();
}
