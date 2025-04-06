using System.Collections.Generic;
using UnityEngine;

public class GameManager : ManagedUpdateBehaviour
{
    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;
    public SphereController Sphere;
    public List<BrickController> Bricks = new List<BrickController>();

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        } else if (Instance != this) 
        {
            Destroy(gameObject);
        }
    }

    public override void UpdateMe()
    {
        bool allGameObjectsOff = false;

        foreach (var item in Bricks)
        {
            if (item.gameObject.activeSelf)
                allGameObjectsOff = true;
        }

        if (!allGameObjectsOff) 
        {
            foreach (var item in Bricks)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}
