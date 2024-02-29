using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    [SerializeField]
    public RifleData Data;

    private Vector3 _localScale;

    [SerializeField]
    private Transform _projectileSpawnPoint;

    private float _timer;
    
    private bool _isReadyToFire;

    public override void Init()
    {
        ObjectMovement bulletObjectMovementComponent = Data.ProjectilePrefab.GetComponent<ObjectMovement>();
        Bullet bullet = Data.ProjectilePrefab.GetComponent<Bullet>();

        bulletObjectMovementComponent.SetAccelerationTime(0);
        bulletObjectMovementComponent.SetDecelerationTime(0);
        bulletObjectMovementComponent.SetChangeDirectionTime(0);
        bulletObjectMovementComponent.SetMaxSpeed(Data.ProjectileSpeed);
        bullet.SetAliveTime(Data.ProjectileLifeTime);
        bullet.SetDamage(Data.Damage);

        gameObject.SetActive(true);
        _localScale = transform.localScale;     
        _isReadyToFire = true;
    }

    public override void Enter()
    {
        Debug.Log($"Weapon {transform.name} enter.");
        Fire();

    }

    public override void RotateWeaponToPoint(Vector2 direction)
    {
        Vector2 directionToCursor = (direction - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(_localScale.x, -_localScale.y, _localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(_localScale.x, _localScale.y, _localScale.z);
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Fire()
    {
        if (_isReadyToFire)
        {
            Instantiate(Data.ProjectilePrefab, _projectileSpawnPoint.position, transform.rotation);
            _isReadyToFire = false;
            _timer = Data.FireRate;
        }
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            _isReadyToFire = true;
            _timer = 0;
        }

    }
    public override void Deinit()
    {
        gameObject.SetActive(false);
    }
}
