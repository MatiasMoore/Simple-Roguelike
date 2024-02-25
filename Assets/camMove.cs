using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMove : MonoBehaviour
{
    [SerializeField]
    private Transform _camTransform;

    [SerializeField]
    private float _dist = 5;

    [SerializeField]
    private float _speed = 1;

    private Vector3 _startPosition;
    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        var ofs = Mathf.Sin(Time.time * _speed) * _dist;

        _camTransform.position = _startPosition + Vector3.up * ofs;
    }
}
