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

    public void StartStreamingLevel(RoomNode rootNode)
    {
        StopStreaming();
        StartCoroutine(StreamLevelCoroutine(rootNode, _focus, _builder));
    }

    public void StopStreaming()
    {
        StopAllCoroutines();
    }

    private IEnumerator StreamLevelCoroutine(RoomNode rootNode, Transform focus, LevelBuilder builder)
    {
        var rooms = rootNode.GetLeaves();
        var sortTask = GetSortedLeavesTask(rooms, focus.position);
        int roomsChecked = 0;
        const int maxRoomsChecked = int.MaxValue;
        while (true)
        {
            if (sortTask.IsCompleted)
            {
                rooms = sortTask.Result;
                sortTask = GetSortedLeavesTask(rooms, focus.position);
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

    private Task<List<RoomNode>> GetSortedLeavesTask(List<RoomNode> roomsOrig, Vector2 centerPos)
    {
        var rooms = new List<RoomNode>(roomsOrig);
        var t = new Task<List<RoomNode>>(() =>
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
