using UnityEngine;

public class BombPowerUp : PowerUpController
{
    public override void PowerUpEffect()
    {
        GameManager.Instance.Player.canXplode = true;
    }
    public void Explosion(SphereController controller, GameManager bricks)
    {
        Vector2 pos = controller.GameObject.transform.position;

        for (int i = 0; i < GameManager.Instance.Bricks.Count; i++)
        {
            var brick = GameManager.Instance.Bricks[i];
            if (brick.GameObject == null || !brick.GameObject.activeSelf) continue;
            Vector2 brickPos = brick.GameObject.transform.position;
            Vector2 brickSize = brick.Size;
            float left = brickPos.x - brickSize.x / 2;
            float right = brickPos.x + brickSize.x / 2;
            float top = brickPos.y + brickSize.y / 2;
            float bottom = brickPos.y - brickSize.y / 2;
            bool exphit = pos.x + GameManager.Instance.PowerUpSettings.Radius > left &&
                       pos.x - GameManager.Instance.PowerUpSettings.Radius < right &&
                       pos.y + GameManager.Instance.PowerUpSettings.Radius > bottom &&
                       pos.y - GameManager.Instance.PowerUpSettings.Radius < top;

            if (exphit)
            {
                brick.CollideReaction();
            }
        }
    }
}