using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPlayerPowerUp : PowerUpController
{    public override void PowerUpEffect()
    {
        GameManager.Instance.LongPlayerEffect();
    }
}
