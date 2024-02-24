using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class LevelGeneratorMono : MonoBehaviour
{
    [Header("Press this to generate a level!")]
    [SerializeField]
    private bool _generateNewLevel = false;
    [SerializeField]
    private bool _abort = false;

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
    private Task<RoomNode> _levelTask;

    private void Start()
    {
        _generator = new LevelGenerator();
    }

    private void FixedUpdate()
    {
        if (_generator.IsStatusUpdated())
            Debug.Log(_generator.GetStatusString());
    }

    private void OnValidate()
    {
        if (_generateNewLevel)
        {
            GenerateNewLevel();
            _generateNewLevel = false;
        }

        if (_abort)
        {
            _generator.AbortTasks();
            _abort = false;
        }
    }

    private void GenerateNewLevel()
    {
        if (_randomiseSeed) 
            _seed = Random.Range(int.MinValue, int.MaxValue);

        _levelTask = _generator.GenerateNewLevel(_initialCenter, _initialWidth, _initialHeight, _iterCount, _cutOffSomeLeafs, _seed);
    }

    private void OnDrawGizmosSelected()
    {
        if (_levelTask != null && _levelTask.IsCompleted)
            LevelGenerator.DebugDrawLevel(_levelTask.Result, _drawRoomEdges, _drawRoomCenter, _drawAllTiles, _drawPerimeterTiles, _drawCenterConnections, _drawRoomConnections);
    }
}
