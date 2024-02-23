using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LevelGeneratorMono : MonoBehaviour
{
    [Header("Press this to generate a level!")]
    [SerializeField]
    private bool _generateNewLevel = false;

    [Header("Generation settings")]
    [SerializeField]
    private int _initialWidth = 50;
    [SerializeField]
    private int _initialHeight = 50;
    [SerializeField]
    private Vector2 _initialCenter = new Vector2(0, 0);
    [SerializeField]
    private int _iterCount = 4;
    [SerializeField]
    private bool _cutOffSomeLeafs = false;
    [SerializeField]
    private int _seed = 0;
    [SerializeField]
    private bool _randomiseSeed = false;

    [Header("Debug draw settings")]
    [SerializeField]
    private bool _drawRoomEdges = true;
    [SerializeField]
    private bool _drawRoomCenter = false;
    [SerializeField]
    private bool _drawAllTiles = false;
    [SerializeField]
    private bool _drawPerimeterTiles = false;
    [SerializeField]
    private bool _drawCenterConnections = false;
    [SerializeField]
    private bool _drawRoomConnections = true;

    private LevelGenerator _generator;


    private void OnValidate()
    {
        if (_generateNewLevel)
        {
            GenerateNewLevel();
            _generateNewLevel = false;
        }
    }

    private void GenerateNewLevel()
    {
        if (_randomiseSeed) 
            _seed = Random.Range(int.MinValue, int.MaxValue);

        _generator = new LevelGenerator(_initialCenter, _initialWidth, _initialHeight, _iterCount, _cutOffSomeLeafs, _seed);
        _generator.GenerateNewLevel();
    }

    private void OnDrawGizmosSelected()
    {
        if (_generator != null)
            _generator.DebugDrawLevel(_drawRoomEdges, _drawRoomCenter, _drawAllTiles, _drawPerimeterTiles, _drawCenterConnections, _drawRoomConnections);
    }
}
