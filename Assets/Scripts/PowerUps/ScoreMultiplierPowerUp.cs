using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMultiplierPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.AddOrRefreshPowerUp(this, 3f);

        GameManager.Instance.ApplyScoreMultiplier(this);
    }
}
