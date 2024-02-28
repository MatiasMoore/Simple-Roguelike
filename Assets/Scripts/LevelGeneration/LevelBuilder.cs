using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform _tileRoot;

    [SerializeField]
    private GameObject _floorTile;

    public void BuildLevelFromBlueprints(List<RoomBlueprint> blueprints)
    {
        DeleteCurrentLevel();
        StartCoroutine(BuildLevelCoroutine(blueprints));
    }

    public IEnumerator BuildRoomCoroutine(RoomBlueprint room)
    {
        var roomObj = new GameObject("Room");
        roomObj.transform.parent = _tileRoot;
        yield return BuildRoom(room, roomObj.transform);
    }

    const int maxTilesPerFrame = 10;

    private IEnumerator BuildRoom(RoomBlueprint room, Transform parent)
    {
        //Place all tiles
        var tilesToFill = room.GetAllTiles();
        int tilesCreated = 0;
        foreach (var tilePos in tilesToFill)
        {
            var createdObj = Instantiate(_floorTile, tilePos, Quaternion.identity, parent);
            var spriteConf = createdObj.GetComponent<SpriteConfigurator>();

            if (spriteConf == null)
                throw new System.Exception("A floor tile doesn't have a sprite configurator!");

            spriteConf.SetId(room.GetId());

            tilesCreated++;
            if (tilesCreated >= maxTilesPerFrame)
            {
                tilesCreated = 0;
                yield return null;
            }
        }

        //Place corridors
        var corridors = room.GetCorridors();
        foreach (var corridor in corridors)
        {
            yield return StartCoroutine(BuildCorridor(corridor, parent));
        }
        _builtRooms.Add((parent.gameObject, room));

        yield break;
    }

    private List<(GameObject, RoomBlueprint)> _builtRooms = new List<(GameObject, RoomBlueprint)>();

    public bool IsRoomBuilt(RoomBlueprint room)
    {
        foreach (var tuple in _builtRooms)
        {
            GameObject obj;
            RoomBlueprint node;
            (obj, node) = tuple;
            if (node == room)
                return true;
        }
        return false;
    }

    public IEnumerator DestroyBuiltRoom(RoomBlueprint room)
    {
        foreach (var tuple in _builtRooms)
        {
            GameObject obj;
            RoomBlueprint node;
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

    public IEnumerator BuildLevelCoroutine(List<RoomBlueprint> blueprints)
    {
        foreach (var room in blueprints)
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

    private IEnumerator BuildCorridor(RoomBlueprint.Corridor corridor, Transform parent)
    {
        int tilesPlaced = 0;
        var corridorPoints = corridor.GetGridPoints();
        for (int i = 0; i < corridorPoints.Count - 1; i++)
        {
            var tiles = corridor.GridPointToTiles(corridorPoints[i]);
            foreach (var tile in tiles)
            {
                var corridorId = corridor.GetId();
                var hitCol = Physics2D.OverlapPoint(tile);

                SpriteConfigurator spriteConf;

                bool update = false;

                if (hitCol != null)
                {
                    spriteConf = hitCol.GetComponent<SpriteConfigurator>();
                    update = true;
                }
                else
                {
                    var createdObj = Instantiate(_floorTile, tile, Quaternion.identity, parent);
                    spriteConf = createdObj.GetComponent<SpriteConfigurator>();
                    tilesPlaced++;
                }

                if (spriteConf == null)
                {
                    Debug.Log("");
                }

                spriteConf.InteractWithId(corridorId);

                if (update)
                    spriteConf.UpdateSprite();

                if (tilesPlaced >= maxTilesPerFrame)
                {
                    tilesPlaced = 0;
                    yield return null;
                }
            }
        }
    }

}
