using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private LevelCreator _levelCreator;

    [SerializeField] 
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        var levelTask = _levelCreator.GenerateNewLevel();

        if (!levelTask.IsCompleted)
            yield return null;

        _levelCreator.StartStreamingLevel(levelTask.Result);

        _player.transform.position = levelTask.Result.GetPlayerSpawn();
    }
}
