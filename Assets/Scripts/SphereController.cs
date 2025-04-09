using UnityEngine;
using UnityEngine.Pool;

public class SphereController : ManagedUpdateBehaviour
{
    Vector2 MoveDirection;
    public Vector2 MoveSpeed;
    public bool InitialLaunch;
    public float Radius;
    public float InitialYOffset;
    PlayerMovement player;
    private bool bounceOnce;
    private Vector2 InitialSpeed;
    public ObjectPool<GameObject> SpherePool;

    public void LaunchDirection(Vector3 Movedirection)
    {
        InitialSpeed = MoveSpeed;
        InitialLaunch = true;
        MoveDirection = Movedirection;
    }

    public override void UpdateMe()
    {
        if (!InitialLaunch)
        {
            if (!player)
                player = GameManager.Instance.Player;

            if (InitialSpeed != Vector2.zero)
                MoveSpeed = InitialSpeed;

            Vector3 playerPos = player.transform.position;
            transform.position = new Vector3(playerPos.x, InitialYOffset);

            if ((Input.GetKeyDown(KeyCode.Space) && !InitialLaunch && GameManager.Instance.ballsInGame == 1) || GameManager.Instance.ballsInGame > 1)
            {
                Vector3 direction = new Vector2(Random.Range(-8.5f, 8.5f), 1);
                LaunchDirection(direction);
            }

            return;
        }

        CalculateCollisions();
    }


    private void CalculateCollisions()
    {
        Vector2 pos = transform.position;


        if (pos.x < GameManager.Instance.XScreenThresshold.x + Radius / 2)
        {
            bounceOnce = false;
            MoveSpeed.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.x + Radius / 2;
        }
        else if (pos.x > GameManager.Instance.XScreenThresshold.y - Radius / 2)
        {
            bounceOnce = false;
            MoveSpeed.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.y - Radius / 2;
        }

        if (pos.y > GameManager.Instance.YScreenThresshold.x - Radius / 2)
        {
            bounceOnce = false;
            MoveSpeed.y *= -1;
            pos.y = GameManager.Instance.YScreenThresshold.x - Radius / 2;
        }

        foreach (var item in GameManager.Instance.Bricks)
        {
            if (!item.gameObject.activeSelf) continue;

            if (Vector3.Distance(transform.position, item.gameObject.transform.position) < 2f) 
            {
                if (pos.y - Radius / 2 < item.transform.position.y + item.Size.y / 2 && pos.y + Radius / 2 > item.transform.position.y - item.Size.y / 2 && pos.x + Radius / 2> item.transform.position.x - item.Size.x / 2 && pos.x - Radius / 2 < item.transform.position.x + item.Size.x / 2) 
                {
                    MoveSpeed.y *= -1;
                    bounceOnce = false;
                    item.CollideReaction();
                }
            }
        }

        if (pos.y - Radius / 2 < player.transform.position.y + player.Size.y / 2 && pos.y + Radius / 2 > player.transform.position.y - player.Size.y / 2 && pos.x + Radius / 2 > player.transform.position.x - player.Size.x / 2 && pos.x - Radius / 2 < player.transform.position.x + player.Size.x / 2 && !bounceOnce)
        {
            bounceOnce = true;


            if (pos.x + Radius < player.transform.position.x - player.Size.x / 4)
            {
                LeftDirection();
                MoveSpeed = new Vector2(Mathf.Abs(MoveSpeed.x), MoveSpeed.y * -1);
            }
            else if (pos.x + Radius > player.transform.position.x - player.Size.x / 4 && pos.x < player.transform.position.x + player.Size.x / 4)
            {
                CenterDirection();
                MoveSpeed = new Vector2(Mathf.Abs(MoveSpeed.x) * -1, MoveSpeed.y * -1);
            }
            else if (pos.x + Radius > player.transform.position.x + player.Size.x / 4)
            {
                RightDirection();
                MoveSpeed = new Vector2(Mathf.Abs(MoveSpeed.x) * -1, MoveSpeed.y * -1);
            }
        }

        if ((pos.y < GameManager.Instance.YScreenThresshold.y + Radius / 2 && GameManager.Instance.ballsInGame == 1) || Input.GetKeyDown(KeyCode.R))
        {
            bounceOnce = false;
            InitialLaunch = !InitialLaunch;
            MoveSpeed = InitialSpeed;
        } 
        else if (pos.y < GameManager.Instance.YScreenThresshold.y + Radius / 2 && GameManager.Instance.ballsInGame > 1) 
        {
            GameManager.Instance.SpherePool.Release(this.gameObject);
            GameManager.Instance.ballsInGame--;
        }

        pos += MoveDirection.normalized * MoveSpeed * Time.deltaTime;

        transform.position = pos;
    }

    public bool CollidesWith() 
    {
        return true;
    }

    private void LeftDirection() 
    {
        MoveDirection = new Vector2(Random.Range(-8.5f, -3.5f), 1);
    }

    private void CenterDirection()
    {
        MoveDirection = new Vector2(Random.Range(-1.5f, 1.5f), 1);
    }

    private void RightDirection()
    {
        MoveDirection = new Vector2(Random.Range(3.5f, 8.5f), 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,Radius);
    }
}
