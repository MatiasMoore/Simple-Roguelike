using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navruntime : MonoBehaviour
{
    public bool update = false;
    public NavMeshPlus.Components.NavMeshSurface _surface;

    private void OnValidate()
    {
        if (update)
        {
            update = false;
            _surface.BuildNavMeshAsync();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
