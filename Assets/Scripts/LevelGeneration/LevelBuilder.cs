using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform _tileRoot;

    [SerializeField]
    private GameObject _floorTile;

    private int _lastID = 0;

    public void BuildLevelFromNodes(RoomNode rootNode)
    {
        DeleteCurrentLevel();
        StartCoroutine(BuildLevelCoroutine(rootNode));
    }

    public IEnumerator BuildRoomCoroutine(RoomNode room)
    {
        var roomObj = new GameObject("Room");
        roomObj.transform.parent = _tileRoot;
        yield return BuildRoom(room, roomObj.transform);
    }

    private IEnumerator BuildRoom(RoomNode room, Transform parent)
    {
        const int maxTilesPerFrame = 10;
        const int maxCorridorsCreated = 2;

        //Place all tiles
        var tilesToFill = room.GetAllGridPoints();
        int tilesCreated = 0;
        foreach (var tilePos in tilesToFill)
        {
            var createdObj = Instantiate(_floorTile, tilePos, Quaternion.identity, parent);
            var spriteConf = createdObj.GetComponent<SpriteConfigurator>();

            if (spriteConf == null)
                throw new System.Exception("A floor tile doesn't have a sprite configurator!");

            spriteConf.SetId(_lastID);

            tilesCreated++;
            if (tilesCreated >= maxTilesPerFrame)
            {
                tilesCreated = 0;
                yield return null;
            }
        }

        _lastID++;

        //Place corridors
        int corridorsCreated = 0;
        var connections = room.GetRoomConnections();
        for (int i = 0; i < connections.Count; i++)
        {
            BuildCorridor(connections[i]._startTilePos, connections[i]._endTilePos, parent, _lastID);
            if (corridorsCreated >= maxCorridorsCreated)
            {
                corridorsCreated = 0;
                yield return null;
            }
        }

        _lastID++;

        _builtRooms.Add((parent.gameObject, room));

        yield break;
    }
    /**/

    private List<(GameObject, RoomNode)> _builtRooms = new List<(GameObject, RoomNode)>();

    public bool IsRoomBuilt(RoomNode room)
    {
        foreach (var tuple in _builtRooms)
        {
            GameObject obj;
            RoomNode node;
            (obj, node) = tuple;
            if (node == room)
                return true;
        }
        return false;
    }

    public IEnumerator DestroyBuiltRoom(RoomNode room)
    {
        foreach (var tuple in _builtRooms)
        {
            GameObject obj;
            RoomNode node;
            (obj, node) = tuple;
            if (node == room)
            {
                _builtRooms.Remove(tuple);
                yield return DestroyRoomObj(obj);
                yield break;
            }
        }
    }

    private IEnumerator DestroyRoomObj(GameObject roomObj)
    {
        int destroyedChildren = 0;
        const int maxDestroyedChildren = 10;
        for (int i = 0; i < roomObj.transform.childCount; i++)
        {
            Destroy(roomObj.transform.GetChild(i).gameObject);

            destroyedChildren++;
            if (destroyedChildren >= maxDestroyedChildren)
            {
                destroyedChildren = 0;
                yield return null;
            }
        }

        Destroy(roomObj);

        yield break;
    }

    public IEnumerator BuildLevelCoroutine(RoomNode rootNode)
    {
        var rooms = rootNode.GetLeaves();
        foreach (var room in rooms)
            yield return BuildRoomCoroutine(room);

        yield break;
    }

    public void DeleteCurrentLevel()
    {
        StopAllCoroutines();
        for (int i = 0; i < _tileRoot.childCount; i++)
        {
            var currentChild = _tileRoot.GetChild(i).gameObject;
            Destroy(currentChild);
        }

        _builtRooms.Clear();
        _lastID = 0;
    }

    private IEnumerator UpdateTileVisuals()
    {
        const int maxTilesPerFrame = 20;

        int tilesUpdated = 0;
        for (int i = 0; i < _tileRoot.childCount; i++)
        {
            var currentChild = _tileRoot.GetChild(i).gameObject;
            var spriteConf = currentChild.GetComponent<SpriteConfigurator>();
            if (spriteConf == null)
                throw new System.Exception("A floor tile doesn't have a sprite configurator!");

            spriteConf.UpdateSprite();

            tilesUpdated++;
            if (tilesUpdated >= maxTilesPerFrame)
            {
                tilesUpdated = 0;
                yield return null;
            }
        }

        yield break;
    }

    private void PlotCorridorLine(Vector2 start, Vector2 end, Transform parent, int corridorId)
    {
        const float step = 0.3f;
        var toEnd = end - start;
        var toEndDir = toEnd.normalized;
        Vector2 lastPos = new Vector2(float.MaxValue, float.MaxValue);
        for (int i = 0; i < toEnd.magnitude / step; i++)
        {
            var currentPos = start + i * step * toEndDir;
            currentPos = new Vector2((int)currentPos.x, (int)currentPos.y);

            if ((currentPos - end).magnitude < 0.1f)
                break;

            if (lastPos.Equals(currentPos))
                continue;

            lastPos = currentPos;

            SpriteConfigurator spriteConf;

            var hitCol = Physics2D.OverlapPoint(currentPos);

            if (hitCol == null)
            {
                var createdObj = Instantiate(_floorTile, currentPos, Quaternion.identity, parent);
                spriteConf = createdObj.GetComponent<SpriteConfigurator>();
            }
            else
            {
                spriteConf = hitCol.GetComponent<SpriteConfigurator>();
            }

            spriteConf.InteractWithId(corridorId);
            spriteConf.UpdateSprite();
        }
    }

    private void BuildCorridor(Vector2 start, Vector2 end, Transform parent, int corridorId)
    {
        SpriteConfigurator endSpriteConf = null;
        var endHit = Physics2D.OverlapPoint(end);
        if (endHit != null)
        {
            endSpriteConf = endHit.GetComponent<SpriteConfigurator>();
            corridorId = endSpriteConf.GetInteractList()[0];
        }

        var middlePoint = new Vector2(start.x, end.y);

        if (endSpriteConf == null)
            middlePoint = new Vector2(end.x, start.y);

        PlotCorridorLine(start, middlePoint, parent, corridorId);
        PlotCorridorLine(middlePoint, end, parent, corridorId);

        if (endSpriteConf != null)
        {
            endSpriteConf.UpdateSprite();
        }
    }
}
