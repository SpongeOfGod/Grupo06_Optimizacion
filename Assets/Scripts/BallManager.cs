using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : ManagedUpdateBehaviourNoMono
{
    List<SphereController> sphereControllers = new List<SphereController>();
    GameObject player;

    public override void UpdateMe()
    {
        sphereControllers = GameManager.Instance.SphereControllers;

        for (int i = 0; i < sphereControllers.Count; i++)
        {
            SphereController controller = sphereControllers[i];
            if (!controller.InitialLaunch) 
            {
                if (!player)
                    player = GameManager.Instance.PlayerRect;

                if (controller.InitialSpeed != Vector2.zero)
                    controller.MoveSpeed = controller.InitialSpeed;

                Vector3 playerPos = player.transform.position;
                controller.sphere.transform.position = new Vector3(playerPos.x, controller.InitialYOffset);

                if ((Input.GetKeyDown(KeyCode.Space) && !controller.InitialLaunch && GameManager.Instance.ballsInGame == 1) || GameManager.Instance.ballsInGame > 1)
                {
                    Vector3 direction = new Vector2(Random.Range(-8.5f, 8.5f), 1).normalized;
                    controller.LaunchDirection(direction);
                }
            }
        }
    }
}
