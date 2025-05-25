using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortPlayerPowerDown : PowerUpController
{
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.ChangeSizePlayerEffect(GameManager.Instance.PowerUpSettings.ShortMultiplier, this);
        GameManager.Instance.PlayAudio(GameManager.Instance.PowerDownClip);
    }
}
