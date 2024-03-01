using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    [SerializeField]
    public RifleData Data;

    [SerializeField]
    private GameObject _weaponHolder;

    [SerializeField]
    private Transform _projectileSpawnPoint;

    private int _currentAmmo;

    [SerializeField]
    private AmmoBar _ammoBar;

    private float _timer;

    private Vector3 _localScale;

    private enum FireState
    {
        readyToFire,
        fireRatePause,
        realoadPause
    }
    private FireState _fireState;

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

        _currentAmmo = Data.AmmoPerMagazine;


        _ammoBar.UpdateAmmo(_currentAmmo, Data.AmmoPerMagazine);
        gameObject.SetActive(true);

        _localScale = transform.localScale;

        _fireState = FireState.readyToFire;
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
            _weaponHolder.transform.localScale = new Vector3(_localScale.x, -_localScale.y, _localScale.z);
        }
        else
        {
            _weaponHolder.transform.localScale = new Vector3(_localScale.x, _localScale.y, _localScale.z);         
        }

        _weaponHolder.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Fire()
    {
        if (_fireState == FireState.readyToFire)
        {
            SpawnProjectile(Data.ProjectilePrefab);
            _currentAmmo--;
            _ammoBar.SetCurrentAmmo(_currentAmmo);
            _timer = 0;
            if (_currentAmmo<=0)
            {
                _fireState = FireState.realoadPause;
                _ammoBar.SetActiveReloadBar(true);
            } else
            {
                _fireState = FireState.fireRatePause;
            }
        }
    }

    private void SpawnProjectile(GameObject projectile)
    {
        GameObject spawnedProjectile = Instantiate(projectile, _projectileSpawnPoint.position, _weaponHolder.transform.rotation); 
        spawnedProjectile.GetComponent<Bullet>().Fire(transform.parent.GetComponent<Rigidbody2D>().velocity);
    }

    private void Update()
    {
        switch (_fireState)
        {
            case FireState.readyToFire:
                break;
            case FireState.fireRatePause:
                FireRatePause();
                break;
            case FireState.realoadPause:
                RealoadPause();
                break;
        }
    }

    public void FireRatePause()
    {
        _timer += Time.deltaTime;
        if (_timer >= Data.FireRate)
        {
            _fireState = FireState.readyToFire;
        }

    }

    public void RealoadPause()
    {
        _timer += Time.deltaTime;
        _ammoBar.UpdateReloadTime(_timer, Data.RealoadeTime);
        if (_timer >= Data.RealoadeTime)
        {
            _currentAmmo = Data.AmmoPerMagazine;
            _ammoBar.SetCurrentAmmo(_currentAmmo);
            _ammoBar.SetActiveReloadBar(false);
            _fireState = FireState.readyToFire;
        }
    }

    public void SetActiveAmmoBar(bool isActive)
    {
        _ammoBar.gameObject.SetActive(isActive);
    }

    public override void Deinit()
    {
        gameObject.SetActive(false);
    }
}
