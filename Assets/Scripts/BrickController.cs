using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : ManagedUpdateBehaviourNoMono
{
    public Vector3 Size = Vector3.one;

    public void CollideReaction() 
    {
        if (Random.value > 0.80 /*&& GameManager.Instance.ballsInGame < 4 && GameManager.Instance.activePowerUps.Count == 0*/)
        {
            GameObject powerup = GameManager.Instance.CreatePowerUp();

            powerup.transform.position = gameObject.transform.position;
        }
        gameObject.SetActive(false);
    }
}
