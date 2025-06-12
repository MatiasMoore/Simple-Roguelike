using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashUpgrade : AbstractUpgrade
{
    [SerializeField]
    private enum Stage
    {
        UpgradeDash,
        UpgradeDashStats
    }

    [SerializeField]
    private Stage _stage;

    [SerializeField]
    private float _dashSpeedMultiplier = 1.2f;
    [SerializeField]
    private float _dashDurationMultiplier = 1.1f;
    [SerializeField]
    private float _dashCooldownMultiplier = 0.9f;
    
    public override void UpplyUpgrade(UpgradeData data)
    {
        if (_stage == Stage.UpgradeDash)
        {
            UpgradeDash(data);
        }
        else if (_stage == Stage.UpgradeDashStats)
        {
            UpgradeDashStats(data);
        }
        
    }

    private void UpgradeDash(UpgradeData data)
    {
        data.Player.GetComponent<PlayerMovementController>().IsDashUpgraded = true;
    }

    private void UpgradeDashStats(UpgradeData data)
    {
        PlayerMovementController playerMovement = data.Player.GetComponent<PlayerMovementController>();
        playerMovement._dashSpeed *= _dashSpeedMultiplier;
        playerMovement._dashDuration *= _dashDurationMultiplier;
        playerMovement._dashCooldown *= _dashCooldownMultiplier;
    }
}
