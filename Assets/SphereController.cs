using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : ManagedUpdateBehaviour
{
    Vector2 MoveDirection;
    public Vector2 MoveSpeed;
    public bool InitialLaunch;
    public float XSize;
    public float YSize;
    PlayerMovement player;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    public void LaunchDirection(Vector3 Movedirection)
    {
        InitialLaunch = true;
        MoveDirection = Movedirection;
    }

    public override void UpdateMe()
    {
        if (!InitialLaunch) 
        {
            Vector3 playerPos = player.transform.position;
            transform.position = new Vector3(playerPos.x, transform.position.y);
            return;
        }

        Vector2 pos = transform.position;


        if (pos.x < GameManager.Instance.XScreenThresshold.x + XSize / 2) 
        {
            MoveSpeed.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.x + XSize / 2;
        } 
        else if (pos.x > GameManager.Instance.XScreenThresshold.y - XSize / 2) 
        {
            MoveSpeed.x *= -1;
            pos.x = GameManager.Instance.XScreenThresshold.y - XSize / 2;
        }

        if (pos.y > GameManager.Instance.YScreenThresshold.x - YSize / 2) 
        {
            MoveSpeed.y *= -1;
            pos.y = GameManager.Instance.YScreenThresshold.x - YSize / 2;
        }

        if (pos.y < player.transform.position.y + player.YSize / 2 && pos.y > player.transform.position.y - player.YSize / 2 && pos.x > player.transform.position.x - player.XSize / 2 && pos.x < player.transform.position.x + player.XSize / 2) 
        {
            MoveSpeed *= -1;
        }

            pos += MoveDirection * MoveSpeed * Time.deltaTime;

        transform.position = pos;
    }
}
