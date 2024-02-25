using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Header: weapon stats
    [Header("Weapon stats")]
    [SerializeField]
    private float _damage;

    [SerializeField]
    private float _fireRate;

    [SerializeField]
    private float _realoadeTime;

    [SerializeField]
    private float _ammoPerMagazine;

    [SerializeField]
    private float _accuracy;

    [SerializeField]
    private float _projectileLifeTime;

    [SerializeField]
    private float _projectileSpeed;

    [SerializeField]
    private float _projectileSize;

    [Header("Weapon technical")]
    private Vector3 _localScale;

    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    private Transform _projectileSpawnPoint;

    public void Init()
    {
        _localScale = transform.localScale;
    }
    public void Enter()
    {
        Debug.Log($"Weapon {transform.name} enter.");
        Instantiate(_projectilePrefab, _projectileSpawnPoint.position, transform.rotation);
    }
    public void RotateWeaponToPoint(Vector2 direction)
    {
        Vector2 directionToCursor = (direction - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
           transform.localScale = new Vector3(_localScale.x, -_localScale.y, _localScale.z);
        } else
        {
            transform.localScale = new Vector3(_localScale.x, _localScale.y, _localScale.z);
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
