using UnityEngine;
using UnityEngine.Pool;

public class SphereController : ManagedUpdateBehaviourNoMono
{
    public GameObject sphere;
    Vector2 MoveDirection;
    public Vector2 MoveSpeed = new Vector2(12, 12);
    public bool InitialLaunch;
    public float Radius = 0.25f;
    public float Mass = 20;
    public float InitialYOffset = -3.93f;
    PlayerMovement player;
    private bool bounceOnce;
    public Vector2 InitialSpeed;
    public ObjectPool<GameObject> SpherePool;

    public void LaunchDirection(Vector3 Movedirection)
    {
        InitialSpeed = MoveSpeed;
        InitialLaunch = true;
        MoveDirection = Movedirection;
    }

    public override void UpdateMe()
    {
        if (!InitialLaunch) return;

        CalculateCollisions();
    }

    private void CalculateCollisions()
    {
        if (sphere == null || !sphere.activeSelf) return;

        Vector2 pos = sphere.transform.position;
        if (pos.x - Radius < GameManager.Instance.XScreenThresshold.x)
        {
            MoveDirection.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.x + Radius;
        }
        else if (pos.x + Radius > GameManager.Instance.XScreenThresshold.y)
        {
            MoveDirection.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.y - Radius;
        }
        if (pos.y + Radius > GameManager.Instance.YScreenThresshold.x)
        {
            MoveDirection.y *= -1;
            pos.y = GameManager.Instance.YScreenThresshold.x - Radius;
        }
        foreach (var brick in GameManager.Instance.Bricks)
        {
            if (!brick.gameObject.activeSelf) continue;
            Vector2 brickPos = brick.transform.position;
            Vector2 brickSize = brick.Size;
            float left = brickPos.x - brickSize.x / 2;
            float right = brickPos.x + brickSize.x / 2;
            float top = brickPos.y + brickSize.y / 2;
            float bottom = brickPos.y - brickSize.y / 2;
            bool hit = pos.x + Radius > left &&
                       pos.x - Radius < right &&
                       pos.y + Radius > bottom &&
                       pos.y - Radius < top;
            if (hit)
            {
                Vector2 prevPos = pos - MoveDirection * MoveSpeed * Time.deltaTime;
                bool fromLeft = prevPos.x + Radius <= left;
                bool fromRight = prevPos.x - Radius >= right;
                bool fromBelow = prevPos.y + Radius <= bottom;
                bool fromAbove = prevPos.y - Radius >= top;
                if (fromLeft || fromRight)
                {
                    MoveDirection.x *= -1;
                }
                else if (fromAbove || fromBelow)
                {
                    MoveDirection.y *= -1;
                }
                else
                {
                    MoveDirection.y *= -1;
                }
                brick.CollideReaction();
                break;
            }
        }
        if (player != null)
        {
            Vector2 playerPos = player.transform.position;
            Vector2 playerSize = player.Size;
            bool hit = pos.x + Radius > playerPos.x - playerSize.x / 2 &&
                       pos.x - Radius < playerPos.x + playerSize.x / 2 &&
                       pos.y - Radius < playerPos.y + playerSize.y / 2 &&
                       pos.y + Radius > playerPos.y - playerSize.y / 2;
            if (hit && !bounceOnce)
            {
                bounceOnce = true;
                float offset = (pos.x - playerPos.x) / (playerSize.x / 2f);
                offset = Mathf.Clamp(offset, -1f, 1f);
                MoveDirection = new Vector2(offset, 1f).normalized;
            }
            else if (!hit)
            {
                bounceOnce = false;
            }
        }
        if ((pos.y - Radius < GameManager.Instance.YScreenThresshold.y && GameManager.Instance.ballsInGame == 1) || Input.GetKeyDown(KeyCode.R))
        {
            bounceOnce = false;
            InitialLaunch = false;
            MoveDirection = Vector2.zero;
        }
        else if (pos.y - Radius < GameManager.Instance.YScreenThresshold.y && GameManager.Instance.ballsInGame > 1)
        {
            GameManager.Instance.SpherePool.Release(sphere);
            GameManager.Instance.ballsInGame--;
            return;
        }

        pos += MoveDirection * MoveSpeed * Time.deltaTime;
        sphere.transform.position = pos;
    }
}
