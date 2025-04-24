using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionManager : ManagedUpdateBehaviourNoMono
{
    private bool bounceOnce;
    List<SphereController> sphereControllers = new List<SphereController>();
    List<PowerUpController> powerUpControllers = new List<PowerUpController>();
    BombPowerUp bomb;
    public override void UpdateMe()
    {
        SphereCollision();

        PowerUpCollision();
    }

    private void SphereCollision()
    {
        sphereControllers = GameManager.Instance.SphereControllers;

        for (int i = 0; i < sphereControllers.Count; i++)
        {
            SphereController controller = sphereControllers[i];

            if (!controller.GameObject) 
            {
                GameManager.Instance.SphereControllers.Remove(controller);
                controller = null;
                continue;
            }

            if (!controller.GameObject.activeSelf) continue;

            Vector2 pos = controller.GameObject.transform.position;

            if (controller.GameObject.transform.position.x - controller.Radius < GameManager.Instance.XScreenThresshold.x)
            {
                controller.MoveDirection.x *= -1;
                pos.x = GameManager.Instance.XScreenThresshold.x + controller.Radius;
            }
            else if (pos.x + controller.Radius > GameManager.Instance.XScreenThresshold.y)
            {
                controller.MoveDirection.x *= -1;
                pos.x = GameManager.Instance.XScreenThresshold.y - controller.Radius;
            }
            if (pos.y + controller.Radius > GameManager.Instance.YScreenThresshold.x)
            {
                controller.MoveDirection.y *= -1;
                pos.y = GameManager.Instance.YScreenThresshold.x - controller.Radius;
            }
     
            foreach (var brick in GameManager.Instance.Bricks)
            {
                if (!brick.GameObject.activeSelf) continue;
                Vector2 brickPos = brick.GameObject.transform.position;
                Vector2 brickSize = brick.Size;
                float left = brickPos.x - brickSize.x / 2;
                float right = brickPos.x + brickSize.x / 2;
                float top = brickPos.y + brickSize.y / 2;
                float bottom = brickPos.y - brickSize.y / 2;
                bool hit = pos.x + controller.Radius > left &&
                           pos.x - controller.Radius < right &&
                           pos.y + controller.Radius > bottom &&
                           pos.y - controller.Radius < top;
                if (hit)
                {
                    if (controller.BombMode)
                    {
                   
                        bomb = controller.bombPowerUp = new BombPowerUp();
                        bomb.Explosion(controller, GameManager.Instance);

                        controller.player.canXplode = false;
                        controller.BombMode = false;

                    }

                    Vector2 prevPos = pos - controller.MoveDirection * controller.MoveSpeed * Time.deltaTime;
                    bool fromLeft = prevPos.x + controller.Radius <= left;
                    bool fromRight = prevPos.x - controller.Radius >= right;
                    bool fromBelow = prevPos.y + controller.Radius <= bottom;
                    bool fromAbove = prevPos.y - controller.Radius >= top;

                  
                    if (!controller.fireBallMode)
                    {
                        if (fromLeft || fromRight)
                        {
                            controller.MoveDirection.x *= -1;
                        }
                        else if (fromAbove || fromBelow)
                        {
                            controller.MoveDirection.y *= -1;
                        }
                        else
                        {
                            controller.MoveDirection.y *= -1;
                        }
                    }
              
                    brick.CollideReaction();
                    break;

                }
            }
            if (controller.player != null)
            {
                Vector2 playerPos = controller.player.GameObject.transform.position;
                Vector2 playerSize = controller.player.Size;
                bool hit = pos.x + controller.Radius > playerPos.x - playerSize.x / 2 &&
                pos.x - controller.Radius < playerPos.x + playerSize.x / 2 &&
                pos.y - controller.Radius < playerPos.y + playerSize.y / 2 &&
                           pos.y + controller.Radius > playerPos.y - playerSize.y / 2;
                if (hit && !bounceOnce)
                {
                    bounceOnce = true;
                    float offset = (pos.x - playerPos.x) / (playerSize.x / 2f);
                    offset = Mathf.Clamp(offset, -1f, 1f);
                    controller.MoveDirection = new Vector2(offset, 1f).normalized;

                    if (GameManager.Instance.Player.fireBallPad) 
                    {
                        controller.fireBallMode = true;
                        controller.trailRenderer.enabled = true;
                    }
                    if (GameManager.Instance.Player.canXplode)
                    {
                        controller.BombMode = true;
                    }
                }
                else if (!hit)
                {
                    bounceOnce = false;
                }
            }
            if ((pos.y - controller.Radius < GameManager.Instance.YScreenThresshold.y && GameManager.Instance.ballsInGame == 1))
            {
                GameManager.Instance.SpawnBallDeathParticles(pos);
                bounceOnce = false;
                controller.InitialLaunch = false;
                controller.MoveDirection = Vector2.zero;
                controller.fireBallMode = false;
                controller.BombMode = false;
                controller.trailRenderer.enabled = false;
                GameManager.Instance.PlayerLifesChanges();
            }
            else if (pos.y - controller.Radius < GameManager.Instance.YScreenThresshold.y && GameManager.Instance.ballsInGame > 1)
            {
                if (controller.GameObject == null || !controller.GameObject.activeSelf) continue;

                controller.GameObject.transform.position = pos;
                controller.fireBallMode = false;
                controller.BombMode = false;
                controller.trailRenderer.enabled = false;
                GameManager.Instance.SpherePool.Release(controller.GameObject);
                GameManager.Instance.ballsInGame--;
                continue;
            }
            controller.GameObject.transform.position = pos;
        }
    }

    private void PowerUpCollision()
    {
        powerUpControllers = GameManager.Instance.activePowerUps;
        PlayerMovement player = GameManager.Instance.Player;

        for (int i = 0; i < powerUpControllers.Count; i++)
        {
            PowerUpController controller = powerUpControllers[i];
            Vector3 pos = controller.GameObject.transform.position;

            if (player != null)
            {
                Vector2 playerPos = player.GameObject.transform.position;
                Vector2 playerSize = player.Size;
                bool hit = pos.x + controller.size.x / 2 > playerPos.x - playerSize.x / 2 &&
                pos.x - controller.size.x / 2 < playerPos.x + playerSize.x / 2 &&
                pos.y - controller.size.y / 2 < playerPos.y + playerSize.y / 2 &&
                           pos.y + controller.size.y / 2 > playerPos.y - playerSize.y / 2;
                if (hit)
                {
                    controller.PowerUpEffect();
                    GameManager.Instance.DestroyPowerUp(controller);
                }
            }
            if (pos.y - controller.size.y / 2 < GameManager.Instance.YScreenThresshold.y)
            {
                GameManager.Instance.DestroyPowerUp(controller);
            }
        }
    }
}
