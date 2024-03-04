using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private LevelCreator _levelCreator;

    [SerializeField]
    private ScreenTransitionController _transition;

    [SerializeField] 
    private GameObject _player;

    [SerializeField]
    private Pedestrian _playerPed;

    // Start is called before the first frame update
    void Start()
    {
        _playerPed.OnDeath += EndGame;
        _levelCreator._levelBuilder.OnRoomObjectCreated += ObjectSpawned;
        _transition.InstaFadeOut();
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        var levelTask = _levelCreator.GenerateNewLevel();

        if (!levelTask.IsCompleted)
            yield return null;

        _player.transform.position = levelTask.Result.GetPlayerSpawn();

        _levelCreator.StartStreamingLevel(levelTask.Result);

        yield return new WaitForSecondsRealtime(3);

        _transition.FadeIn(1f);
    }

    private void NextLevel()
    {
        Debug.Log("Starting next level!");
        StartCoroutine(NextLevelCoroutine());
    }

    private IEnumerator NextLevelCoroutine()
    {
        _transition.FadeOut(1f);
        yield return new WaitForSecondsRealtime(2);
        _levelCreator.StopStreamingLevel();
        _levelCreator.DeleteCurrentLevel();
        StartCoroutine(StartLevel());
    }

    private void ObjectSpawned(GameObject newObject)
    {
        var ped = newObject.GetComponent<Pedestrian>();
        if (ped != null && ped.GetPedType() == Pedestrian.PedType.boss)
        {
            ped.OnDeath += NextLevel;
        }
    }

    private void EndGame()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
