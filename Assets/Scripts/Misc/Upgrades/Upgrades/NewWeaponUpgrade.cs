using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWeaponUpgrade : AbstractUpgrade
{
    [SerializeField]
    private GameObject _weaponPrefab;

    public override bool UpplyUpgrade(UpgradeData data)
    {

        PlayerWeaponController weaponController = data.Player.GetComponent<PlayerWeaponController>();
        GameObject weapon = Instantiate(_weaponPrefab, data.Player.transform);
        weaponController.AddNewWeapon(weapon);

        return true;
    }
}
