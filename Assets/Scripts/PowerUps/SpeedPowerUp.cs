using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.ApplySpeedBoostBuff();
    }
}
