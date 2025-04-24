using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortPlayerPowerDown : PowerUpController
{
    float sizeMultiplier = 0.5f;
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.ChangeSizePlayerEffect(sizeMultiplier, this);
    }
}
