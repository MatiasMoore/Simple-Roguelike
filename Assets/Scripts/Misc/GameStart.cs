using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    LevelCreator _levelGenerator;

    // Start is called before the first frame update
    void Start()
    {
        _levelGenerator.GenerateAndStreamLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
