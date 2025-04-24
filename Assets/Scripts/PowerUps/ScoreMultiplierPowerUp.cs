using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMultiplierPowerUp : PowerUpController
{
    float newMultiplier = 2f;
    float duration = 10f;

    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.AddOrRefreshPowerUp(this, 3f);

        GameManager.Instance.ApplyScoreMultiplier(newMultiplier, duration, this);
    }
}
