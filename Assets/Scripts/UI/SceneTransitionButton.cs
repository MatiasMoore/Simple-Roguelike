using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionButton : MonoBehaviour
{
    [SerializeField]
    private int _sceneIndex;

    public void TransitionToSelectedScene()
    {
        SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);
    }
}
