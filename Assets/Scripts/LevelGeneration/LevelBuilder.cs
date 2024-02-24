using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform _tileRoot;

    [SerializeField]
    private GameObject _floorTile;

    public void BuildLevelFromNodes(RoomNode rootNode)
    {
        StartCoroutine(BuildLevelCoroutine(rootNode));
    }

    public IEnumerator BuildLevelCoroutine(RoomNode rootNode)
    {
        DeleteCurrentLevel();

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

        foreach (var room in rooms)
        {
            var connections = room.GetRoomConnections();
            for (int i = 0; i < connections.Count; i++)
            {
                BuildCorridor(connections[i]._startTilePos, connections[i]._endTilePos, i);
                yield return null;
            }
        }

        StartCoroutine(UpdateTileVisuals());

        yield break;
    }

    public void DeleteCurrentLevel()
    {
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

    private void BuildCorridor(Vector2 start, Vector2 end, int corridorId)
    {
        const float step = 0.3f;
        var toEnd = end - start;
        var toEndDir = toEnd.normalized;
        for (int i = 0; i < toEnd.magnitude / step; i++)
        {
            var currentPos = start + i * step * toEndDir;
            currentPos = new Vector2((int)currentPos.x, (int)currentPos.y);

            SpriteConfigurator spriteConf;

            var hitCol = Physics2D.OverlapPoint(currentPos);

            if (hitCol == null)
            {
                var createdObj = Instantiate(_floorTile, currentPos, Quaternion.identity, _tileRoot);
                spriteConf = createdObj.GetComponent<SpriteConfigurator>();
            }
            else
            {
                spriteConf = hitCol.GetComponent<SpriteConfigurator>();
            }

            spriteConf.InteractWithId(corridorId);
        }
    }
}
