using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrade : AbstractUpgrade
{
    [SerializeField]
    private float speedIncrease = 0f;

    [SerializeField]
    private float hpIncrease = 0;

    public override bool UpplyUpgrade(UpgradeData data)
    {
        Pedestrian playerPedestrian = data.Player.GetComponent<Pedestrian>();

        playerPedestrian.SetMaxHp(playerPedestrian.GetMaxHP() + hpIncrease);
        playerPedestrian.SetHp(playerPedestrian.GetHp() + hpIncrease);

        ObjectMovement playerMovement = data.Player.GetComponent<ObjectMovement>();
        playerMovement.SetMaxSpeed(playerMovement.GetMaxSpeed() + speedIncrease);

        return true;
    }
}
