using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPlayerPowerUp : PowerUpController
{
    float sizeMultiplier = 2.5f;
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.LongPlayerEffect(sizeMultiplier);
    }
}
