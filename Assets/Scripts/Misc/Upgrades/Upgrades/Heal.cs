using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : AbstractUpgrade
{
    public override bool UpplyUpgrade(UpgradeData data)
    {
        Pedestrian playerPedestrian = data.Player.GetComponent<Pedestrian>();
        playerPedestrian.SetHp((int)playerPedestrian.GetMaxHP());

        return true;
    }


}
