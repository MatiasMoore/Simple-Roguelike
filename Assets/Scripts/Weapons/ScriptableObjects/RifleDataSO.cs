using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New rifle data", menuName = "ScriptableObjects/Rifle", order = 1)]
[Serializable]
public class RifleData : WeaponDataSO
{
    [SerializeField]
    public float FireRate;

    [SerializeField]
    public float RealoadeTime;

    [Min(1),SerializeField]
    public int AmmoPerMagazine;

    [SerializeField]
    public float Accuracy;

    [SerializeField]
    public float ProjectileLifeTime;

    [SerializeField]
    public float ProjectileSpeed;

    [SerializeField]
    public GameObject ProjectilePrefab;

    [SerializeField]
    public ScreenShakeProfile ScreenShakeProfile;
}
