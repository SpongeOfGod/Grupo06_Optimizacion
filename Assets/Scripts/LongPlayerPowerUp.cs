using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPlayerPowerUp : PowerUpController
{
    float sizeMultiplier = 3.5f;
    public override void PowerUpEffect()
    {
        GameManager.Instance.LongPlayerEffect(sizeMultiplier);
    }
}
