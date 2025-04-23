using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : ManagedUpdateBehaviourNoMono
{
    public Vector3 Size = Vector3.one;
    public PowerUpController powerUp;

    public void CollideReaction()
    {
        if (powerUp != null)
        {
            powerUp.GameObject.SetActive(true);
            powerUp.GameObject.transform.SetParent(GameManager.Instance.powerUpParent.transform);
            powerUp.speedScale = 5f;
        }

        GameManager.Instance.IncreaseScore(50);

        powerUp = null;
        gameObject.SetActive(false);
    }
}
   
