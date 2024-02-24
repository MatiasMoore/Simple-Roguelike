using System.Collections;
using System.Collections.Generic;
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

    private IEnumerator BuildRoom(RoomNode room)
    {
        const int maxTilesPerFrame = 10;
        const int maxCorridorsCreated = 2;

        var roomObj = new GameObject("Room");
        roomObj.transform.parent = _tileRoot;

        //Place all tiles
        var tilesToFill = room.GetAllGridPoints();
        int tilesCreated = 0;
        foreach (var tilePos in tilesToFill)
        {
            var createdObj = Instantiate(_floorTile, tilePos, Quaternion.identity, roomObj.transform);
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
            BuildCorridor(connections[i]._startTilePos, connections[i]._endTilePos, roomObj.transform, _lastID);
            if (corridorsCreated >= maxCorridorsCreated)
            {
                corridorsCreated = 0;
                yield return null;
            }
        }

        _lastID++;

        yield break;
    }
    /**/
    public IEnumerator BuildLevelCoroutine(RoomNode rootNode)
    {
        Debug.Log("Balls");
        var rooms = rootNode.GetLeaves();

        foreach (var room in rooms)
        {
            yield return StartCoroutine(BuildRoom(room));
        }

        yield break;
    }
    /**/

    /**
    public IEnumerator BuildLevelCoroutine(RoomNode rootNode)
    {
        const int maxTilesPerFrame = 10;

        var rooms = rootNode.GetLeaves();
        for (int i = 0; i < rooms.Count; i++)
        {
            var tilesToFill = rooms[i].GetAllGridPoints();
            int tilesCreated = 0;
            foreach (var tilePos in tilesToFill)
            {
                var createdObj = Instantiate(_floorTile, tilePos, Quaternion.identity, _tileRoot);
                var spriteConf = createdObj.GetComponent<SpriteConfigurator>();

                if (spriteConf == null)
                    throw new System.Exception("A floor tile doesn't have a sprite configurator!");

                spriteConf.SetId(i);

                tilesCreated++;
                if (tilesCreated >= maxTilesPerFrame)
                {
                    tilesCreated = 0;
                    yield return null;
                }
            }
        }

        const int maxCorridorsCreated = 10;
        int corridorsCreated = 0;
        foreach (var room in rooms)
        {
            var connections = room.GetRoomConnections();
            for (int i = 0; i < connections.Count; i++)
            {
                BuildCorridor(connections[i]._startTilePos, connections[i]._endTilePos, _tileRoot, i);
                if (corridorsCreated >= maxCorridorsCreated)
                {
                    corridorsCreated = 0;
                    yield return null;
                }
            }
        }

        StartCoroutine(UpdateTileVisuals());

        yield break;
    }
    /**/
    public void DeleteCurrentLevel()
    {
        StopAllCoroutines();
        for (int i = 0; i < _tileRoot.childCount; i++)
        {
            var currentChild = _tileRoot.GetChild(i).gameObject;
            Destroy(currentChild);
        }
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

    private void BuildCorridor(Vector2 start, Vector2 end, Transform parent, int corridorId)
    {
        SpriteConfigurator endSpriteConf = null;
        var endHit = Physics2D.OverlapPoint(end);
        if (endHit != null)
        {
            endSpriteConf = endHit.GetComponent<SpriteConfigurator>();
            corridorId = endSpriteConf.GetInteractList()[0];
        }

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

        if (endSpriteConf != null)
        {
            endSpriteConf.UpdateSprite();
        }
    }
}
