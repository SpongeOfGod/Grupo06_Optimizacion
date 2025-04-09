using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : ManagedUpdateBehaviour
{
    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;
    public List<BrickController> Bricks = new List<BrickController>();
    public GameObject prefabBall;

    ObjectPool<GameObject> SpherePool;
    public int ballsInGame;

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

        SpherePool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);
        ballsInGame++;
        SpherePool.Get();
    }

    private void OnDestroyPoolObject(GameObject Gobject)
    {
        Destroy(Gobject);
    }

    private void OnReturnedToPool(GameObject Gobject)
    {
        Gobject.SetActive(false);
    }

    private void OnTakeFromPool(GameObject Gobject)
    {
        if (Gobject != null) 
        {
            Gobject.SetActive(true);
            Gobject.transform.position = transform.position;
            SphereController sphereController = Gobject.GetComponent<SphereController>();
            sphereController.InitialLaunch = false;
        }
    }

    private GameObject CreatePooledItem()
    {
        GameObject ball;

        ball = Instantiate(prefabBall, transform.position, Quaternion.identity);

        SphereController sphereController = ball.GetComponent<SphereController>();
        sphereController.SpherePool = SpherePool;

        return ball;
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

        if (Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            SpherePool.Get();
            SpherePool.Get();

            ballsInGame++;
            ballsInGame++;
        }
    }
}
