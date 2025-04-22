using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BombPowerUp : PowerUpController
{
    private float radius = 0.02f;
   
    Vector2 dir;


    public override void PowerUpEffect()
    {
        GameManager.Instance.SphereControllers[1].CanXplode = true;
    }

    public void Explosion( SphereController sphereController)
    {
        foreach (var brick in GameManager.Instance.Bricks)
        {
            if (!brick.GameObject.activeSelf) continue;
            Vector2 brickPos = brick.GameObject.transform.position;
            Vector2 brickSize = brick.Size;
            float left = brickPos.x - brickSize.x / 2;
            float right = brickPos.x + brickSize.x / 2;
            float top = brickPos.y + brickSize.y / 2;
            float bottom = brickPos.y - brickSize.y / 2;
            var hits = sphereController.GameObject.transform.position.x + radius > left &&
            sphereController.GameObject.transform.position.x - radius < right &&
                         sphereController.GameObject.transform.position.y + radius > bottom &&
                          sphereController.GameObject.transform.position.y - radius < top;
            
         
            brick.CollideReaction();
          
            
            sphereController.CanXplode = false;

        }






    }
}
