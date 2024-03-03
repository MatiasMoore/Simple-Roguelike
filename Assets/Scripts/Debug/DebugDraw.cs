using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugDraw
{
    public static void DrawRectangle(Vector3 upperLeft, Vector3 upperRight, Vector3 lowerRight, Vector3 lowerLeft, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLineStrip(new Vector3[] { upperLeft, upperRight, lowerRight, lowerLeft }, true);
    }

    public static void DrawCross(Vector2 pos, float length, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(pos + new Vector2(-length, 0), pos + new Vector2(length, 0));
        Gizmos.DrawLine(pos + new Vector2(0, -length), pos + new Vector2(0, length));
    }

    public static void DrawCell(Vector3 pos, float size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(pos, new Vector3(size, size, 0));
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);
    }

    public static void DrawSphere(Vector2 pos, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(pos, radius);
    }
}
