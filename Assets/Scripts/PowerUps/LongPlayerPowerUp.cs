using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPlayerPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.ChangeSizePlayerEffect(GameManager.Instance.PowerUpSettings.LongMultiplier, this);
    }
}
