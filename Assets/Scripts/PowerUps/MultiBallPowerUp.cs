using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBallPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.MultipleBallEffect();
    }
}
