using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.U2D;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private NavMeshPlus.Components.NavMeshSurface _navMesh;

    [SerializeField]
    private Transform _roomRoot;

    [SerializeField]
    private Transform _permanentObjects;

    [SerializeField]
    private GameObject _floorTile;

    [SerializeField]
    private GameObject _navMeshWalkable;

    public UnityAction<GameObject> OnRoomObjectCreated;

    private Dictionary<RoomBlueprint, GameObject> _builtRooms = new Dictionary<RoomBlueprint, GameObject>();

    public void SetNavMeshWalkable(Vector2 center, float width, float height)
    {
        _navMeshWalkable.transform.localScale = new Vector2(width, height);
        _navMeshWalkable.transform.position = center;
    }

    public void BuildLevelFromBlueprints(Level level)
    {
        DeleteCurrentLevel();
        StartCoroutine(BuildLevelCoroutine(level));
    }

    public IEnumerator BuildRoomCoroutine(RoomBlueprint room)
    {
        var roomObj = new GameObject("Room");
        roomObj.transform.parent = _roomRoot;
        yield return BuildRoom(room, roomObj.transform);
    }

    private void AddBuiltRoom(RoomBlueprint room, GameObject obj)
    {
        _builtRooms.Add(room, obj);
        RebuildNavMesh();
    }

    private void RemoveBuiltRoom(RoomBlueprint room)
    {
        _builtRooms.Remove(room);
        RebuildNavMesh();
    }

    private void ClearBuiltRooms()
    {
        _builtRooms.Clear();
        RebuildNavMesh();
    }

    private AsyncOperation _buildNavMesh;
    private AsyncOperation _updateNavMesh;

    private void RebuildNavMesh()
    {
        if (_buildNavMesh == null)
        {
            Debug.Log("Rebuilding mesh!");
            _buildNavMesh = _navMesh.BuildNavMeshAsync();
        }
        else if ((_buildNavMesh != null && _buildNavMesh.isDone) && (_updateNavMesh == null || _updateNavMesh.isDone))
        {
            Debug.Log("Updating mesh!");
            _updateNavMesh = _navMesh.UpdateNavMesh(_navMesh.navMeshData);
        }
        
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

        //Build all objects
        var objs = room.GetRoomObjects();
        foreach (var obj in objs)
        {
            var createdObj = obj.Build(parent, _permanentObjects);
            if (createdObj != null)
                OnRoomObjectCreated?.Invoke(createdObj);
        }

        AddBuiltRoom(room, parent.gameObject);

        yield break;
    }

    public List<RoomBlueprint> GetBuiltRooms()
    {
        return _builtRooms.Keys.ToList();
    }

    public bool IsRoomBuilt(RoomBlueprint room)
    {
        return _builtRooms.Keys.Contains(room);
    }

    public IEnumerator DestroyBuiltRoom(RoomBlueprint room)
    {
        yield return DestroyRoomObj(_builtRooms[room]);
        RemoveBuiltRoom(room);
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

    public IEnumerator BuildLevelCoroutine(Level level)
    {
        foreach (var room in level.GetRooms())
            yield return BuildRoomCoroutine(room);

        yield break;
    }

    public void DeleteCurrentLevel()
    {
        StopAllCoroutines();
        for (int i = 0; i < _roomRoot.childCount; i++)
        {
            var currentChild = _roomRoot.GetChild(i).gameObject;
            Destroy(currentChild);
        }

        for (int i = 0; i < _permanentObjects.childCount; i++)
        {
            var currentChild = _permanentObjects.GetChild(i).gameObject;
            Destroy(currentChild);
        }

        ClearBuiltRooms();
    }

    private IEnumerator UpdateTileVisuals()
    {
        const int maxTilesPerFrame = 20;

        int tilesUpdated = 0;
        for (int i = 0; i < _roomRoot.childCount; i++)
        {
            var currentChild = _roomRoot.GetChild(i).gameObject;
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
                var hitCol = Physics2D.OverlapPoint(tile, 1 << LayerMask.NameToLayer("Floor"));

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
