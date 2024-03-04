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
        inactive,
        readyToFire,
        fireRatePause,
        realoadPause
    }
    private FireState _fireState = FireState.inactive;

    private void Start()
    {
        _currentAmmo = Data.AmmoPerMagazine;
        _ammoBar.UpdateAmmo(_currentAmmo, Data.AmmoPerMagazine);
    }

    public override void Init()
    {
        _ammoBar.UpdateAmmo(_currentAmmo, Data.AmmoPerMagazine);
        gameObject.SetActive(true);

        _localScale = _weaponHolder.transform.localScale;

        _timer = 0;

        _fireState = FireState.readyToFire;
    }

    public override void Enter()
    {
        if (_fireState == FireState.inactive)
        {
            Debug.LogError($"Weapon {gameObject.name} is inactive");
            return;
        }

        Fire();
    }

    public override void RotateWeaponToPoint(Vector2 pos)
    {
        Vector2 toPos = pos - (Vector2)transform.position;

        var desiredQuat = Quaternion.LookRotation(toPos);

        desiredQuat *= Quaternion.AngleAxis(-90, Vector3.up);
        
        _weaponHolder.transform.rotation = desiredQuat;

    }

    private void Fire()
    {
        if (_fireState == FireState.readyToFire)
        {
            SpawnProjectile(Data.ProjectilePrefab);
            _currentAmmo--;
            RecoilShake();
            _ammoBar.SetCurrentAmmo(_currentAmmo);
            _timer = 0;
            _fireState = FireState.fireRatePause;

        }
    }

    private void RecoilShake()
    {
        if (Data.RecoilShake != null)
        {
            Debug.Log("Shake");
            Data.RecoilShake.sourceDeafaultVelocity = _weaponHolder.transform.right * -1;
            ScreenShaker.Instance.ShakeScreen(Data.RecoilShake);
        }
        return;
    }

    private void SpawnProjectile(GameObject projectile)
    {
        ObjectMovement bulletObjectMovementComponent = Data.ProjectilePrefab.GetComponent<ObjectMovement>();
        Bullet bullet = Data.ProjectilePrefab.GetComponent<Bullet>();

        bulletObjectMovementComponent.SetAccelerationTime(0);
        bulletObjectMovementComponent.SetDecelerationTime(0);
        bulletObjectMovementComponent.SetChangeDirectionTime(0);
        bulletObjectMovementComponent.SetMaxSpeed(Data.ProjectileSpeed);
        bullet.SetAliveTime(Data.ProjectileLifeTime);
        bullet.SetDamage(Data.Damage);
        bullet.SetWhatIDamage(Data.WhatDamage);
        bullet.SetWhatDestroysMe(Data.ByWhatDestroys);

        var angle = _weaponHolder.transform.rotation * Quaternion.AngleAxis(Random.Range(-Data.Accuracy, Data.Accuracy), new Vector3(0, 0, 1));
        var position = _projectileSpawnPoint.position;
        position.z = -1;

        Instantiate(projectile, position, angle);
    }

    private void FixedUpdate()
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
            if (_currentAmmo <= 0)
            {
                _fireState = FireState.realoadPause;
                _ammoBar.SetActiveReloadBar(true);
            } else
            {
                _fireState = FireState.readyToFire;
            }       
        }

    }

    public void RealoadPause()
    {
        _ammoBar.SetActiveReloadBar(true);
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

    public void Reload()
    {
        _fireState = FireState.realoadPause;
    }

    public override void Deinit()
    {
        _ammoBar.SetActiveReloadBar(false);
        _weaponHolder.transform.localScale = _localScale;
        _fireState = FireState.inactive;
        gameObject.SetActive(false);
    }
}
