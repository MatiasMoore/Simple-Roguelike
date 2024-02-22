using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LevelGeneratorMono : MonoBehaviour
{
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

    [SerializeField]
    private bool _debugDrawRoomEdges = true;
    [SerializeField]
    private bool _debugDrawRoomCenter = false;
    [SerializeField]
    private bool _debugDrawAllTiles = false;
    [SerializeField]
    private bool _debugDrawPerimeterTiles = false;
    [SerializeField]
    private bool _debugDrawRoomConnections = false;
    [SerializeField]
    private bool _debugDrawRoomCorridors = true;

    private LevelGenerator _generator;

    private void OnEnable()
    {
        if (_randomiseSeed) 
            _seed = Random.Range(int.MinValue, int.MaxValue);

        _generator = new LevelGenerator(_initialCenter, _initialWidth, _initialHeight, _iterCount, _cutOffSomeLeafs, _seed);
        _generator.GenerateNewLevel();
    }

    void OnDrawGizmosSelected()
    {
        if (_generator != null)
            _generator.DebugDrawLevel(_debugDrawRoomEdges, _debugDrawRoomCenter, _debugDrawAllTiles, _debugDrawPerimeterTiles, _debugDrawRoomConnections, _debugDrawRoomCorridors);
    }
}
