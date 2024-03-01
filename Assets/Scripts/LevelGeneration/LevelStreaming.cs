using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelStreaming : MonoBehaviour
{
    [Header("Transform to stream a level around")]
    [SerializeField]
    private Transform _focus;
    [SerializeField]
    private float _buildWidth = 10;
    [SerializeField]
    private float _buildHeight = 10;
    [SerializeField]
    private float _destroyWidth = 20;
    [SerializeField]
    private float _destroyHeight = 20;

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

    private void OnDrawGizmosSelected()
    {
        var rect = new Rectangle(_focus.position, _buildWidth, _buildHeight);
        DebugDraw.DrawRectangle(rect.GetUpperLeft(), rect.GetUpperRight(), rect.GetLowerRight(), rect.GetLowerLeft(), Color.green);
        rect = new Rectangle(_focus.position, _destroyWidth, _destroyWidth);
        DebugDraw.DrawRectangle(rect.GetUpperLeft(), rect.GetUpperRight(), rect.GetLowerRight(), rect.GetLowerLeft(), Color.red);
    }

    private IEnumerator StreamLevelCoroutine(List<RoomBlueprint> blueprints, Transform focus, LevelBuilder builder)
    {
        Rectangle buildRect, destroyRect;

        var rooms = new List<RoomBlueprint>(blueprints);
        while (true)
        {
            buildRect = new Rectangle(_focus.position, _buildWidth, _buildHeight);
            foreach (var room in rooms)
            {
                //Building rooms
                if (room.DoesIntersect(buildRect) && !builder.IsRoomBuilt(room))
                {
                    yield return StartCoroutine(builder.BuildRoomCoroutine(room));
                }
            }

            destroyRect = new Rectangle(_focus.position, _destroyWidth, _destroyHeight);
            var builtRooms = builder.GetBuiltRooms();
            foreach (var room in builtRooms)
            {
                //Destroying rooms
                if (!room.DoesIntersect(destroyRect))
                {
                    yield return StartCoroutine(builder.DestroyBuiltRoom(room));
                }
            }

            yield return null;
        }
    }
}
