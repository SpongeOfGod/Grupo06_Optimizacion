using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.EnableFireBall();
    }
}
