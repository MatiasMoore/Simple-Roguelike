using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [Header("Press this to generate a level blueprint!")]
    [SerializeField]
    private bool _generateNewLevel = false;
    [SerializeField]
    private bool _abort = false;

    [Header("Press this to build a level using the blueprint!")]
    [SerializeField]
    private bool _buildLevel = false;
    [SerializeField]
    private bool _deleteCurrentLevel = false;

    [Header("Press this to stream a level using the blueprint!")]
    [SerializeField]
    private bool _streamLevel = false;
    [SerializeField]
    private bool _stopStreamingLevel = false;

    [Header("Generation settings")]
    [SerializeField]
    private bool _randomiseSeed = true;
    [SerializeField]
    private LevelGenerator.LevelGenerationData _generationData;

    [Header("Debug draw settings")]
    [SerializeField]
    private bool _drawRoomEdges = true;
    [SerializeField]
    private bool _drawRoomCenter = false;
    [SerializeField]
    private bool _drawAllTiles = false;
    [SerializeField]
    private bool _drawAllGridPoints = false;
    [SerializeField]
    private bool _drawPerimeterGridPoints = false;
    [SerializeField]
    private bool _drawCenterConnections = false;
    [SerializeField]
    private bool _drawRoomConnections = true;

    private LevelGenerator _generator;
    private Task<Level> _levelTask;

    [Header("Helper classes")]
    [SerializeField]
    private LevelBuilder _levelBuilder;
    [SerializeField]
    private LevelStreaming _levelStreaming;

    private void Awake()
    {
        _generator = new LevelGenerator();
    }

    private void Update()
    {
        if (_levelTask != null && _levelTask.IsFaulted)
            Debug.LogError(_levelTask.Exception);
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

        if (_buildLevel)
        {
            if (_levelTask != null && _levelTask.IsCompleted)
                _levelBuilder.BuildLevelFromBlueprints(_levelTask.Result);
            _buildLevel = false;
        }

        if (_deleteCurrentLevel)
        {
            _levelBuilder.DeleteCurrentLevel();
            _deleteCurrentLevel = false;
        }

        if (_streamLevel)
        {
            if (_levelTask != null && _levelTask.IsCompleted)
                _levelStreaming.StartStreamingLevel(_levelTask.Result);
            _streamLevel = false;
        }

        if (_stopStreamingLevel)
        {
            _levelStreaming.StopStreaming();
            _stopStreamingLevel = false;
        }
    }

    public Task<Level> GenerateNewLevel()
    {
        if (_randomiseSeed) 
            _generationData.seed = Random.Range(int.MinValue, int.MaxValue);

        var gap = _generationData.allignmentGrid.GetGap();
        LevelGenerator.LevelGenerationData genData = _generationData;
        genData.startingArea = new Rectangle(gap * _generationData.startingArea.GetCenter(), 
            gap * _generationData.startingArea.GetWidth(), 
            gap * _generationData.startingArea.GetHeight());
        
        _levelTask = _generator.GenerateNewLevel(genData);
        return _levelTask;
    }

    public void StartStreamingLevel(Level level)
    {
        _levelStreaming.StartStreamingLevel(level);
    }

    private void OnDrawGizmosSelected()
    {
        if (_levelTask != null && _levelTask.IsCompleted)
            _generator.DebugDrawLevel(_levelTask.Result, _drawRoomEdges, _drawRoomCenter, _drawAllGridPoints, _drawPerimeterGridPoints, _drawAllTiles, _drawCenterConnections, _drawRoomConnections);
    }
}
