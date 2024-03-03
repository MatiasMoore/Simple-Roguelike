using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnableObject
{
    [SerializeField] 
    public GameObject _spawnObject;

    [SerializeField]
    public Vector3 _offset;
}
