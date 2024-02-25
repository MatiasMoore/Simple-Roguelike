using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponDataSO : ScriptableObject
{
    [SerializeField]
    public string weaponName;

    [SerializeField]
    public string weaponDescription;

    [SerializeField]
    public Sprite weaponIcon;

    [SerializeField]
    public GameObject weaponPrefab;
    
    [SerializeField]
    public float Damage;


}
