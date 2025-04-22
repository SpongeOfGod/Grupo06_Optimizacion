using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUpController
{
    float speedIncreaseAmount = 3f;
    float duration = 5f;

    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.ApplySpeedBoostBuff(speedIncreaseAmount, duration);
    }
}
