using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelStreaming : MonoBehaviour
{
    [Header("Transform to stream a level around")]
    [SerializeField]
    private Transform _focus;

    [Header("Used to actually build the level")]
    [SerializeField]
    private LevelBuilder _builder;

    public void StartStreamingLevel(List<RoomBlueprint> blueprints)
    {
        StopStreaming();
        StartCoroutine(StreamLevelCoroutine(blueprints, _focus, _builder));
    }

    public void StopStreaming()
    {
        StopAllCoroutines();
    }

    private IEnumerator StreamLevelCoroutine(List<RoomBlueprint> blueprints, Transform focus, LevelBuilder builder)
    {
        var rooms = new List<RoomBlueprint>(blueprints);
        var sortTask = GetSortedRooms(rooms, focus.position);
        int roomsChecked = 0;
        const int maxRoomsChecked = int.MaxValue;
        while (true)
        {
            if (sortTask.IsCompleted)
            {
                rooms = sortTask.Result;
                sortTask = GetSortedRooms(rooms, focus.position);
                Debug.Log("Sorted!");
            }
            else
                Debug.Log("Waiting...");

            for (int i = 0; i < rooms.Count; i++)
            {
                //Building rooms
                if (i < 10)
                {
                    if (!builder.IsRoomBuilt(rooms[i]))
                    {
                        yield return StartCoroutine(builder.BuildRoomCoroutine(rooms[i]));
                    }
                }
                //Removing rooms
                else if (i > 20 && builder.IsRoomBuilt(rooms[i]))
                {
                    yield return builder.DestroyBuiltRoom(rooms[i]);
                }

                roomsChecked++;
                if (roomsChecked >= maxRoomsChecked)
                {
                    roomsChecked = 0;
                    yield return null;
                }

            }
            yield return null;
        }
    }

    private Task<List<RoomBlueprint>> GetSortedRooms(List<RoomBlueprint> roomsOrig, Vector2 centerPos)
    {
        var rooms = new List<RoomBlueprint>(roomsOrig);
        var t = new Task<List<RoomBlueprint>>(() =>
        {
            rooms.Sort((a, b) =>
            {
                var aDist = (a.GetCenter() - centerPos).magnitude;
                var bDist = (b.GetCenter() - centerPos).magnitude;
                return aDist.CompareTo(bDist);
            });

            return rooms;
        });
        t.Start();
        return t;
    }
}
