using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject
{
    private GameObject _prefab;

    private Vector3 _position;

    private bool _buildOnce;
    private bool _wasBuilt = false;

    public RoomObject(GameObject prefab, Vector3 position, bool buildOnce)
    {
        _prefab = prefab;
        _position = position;
        _buildOnce = buildOnce;
    }

    public GameObject Build(Transform parent)
    {
        if (_buildOnce && _wasBuilt)
            return null;

        return UnityEngine.Object.Instantiate(_prefab, _position, Quaternion.identity, parent);
    }

}
