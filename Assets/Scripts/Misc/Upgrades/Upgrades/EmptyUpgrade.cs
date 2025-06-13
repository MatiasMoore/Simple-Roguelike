using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyUpgrade : AbstractUpgrade
{
    public override bool UpplyUpgrade(UpgradeData data)
    {
        return true;
    }
}
