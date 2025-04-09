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
    public GameObject prefabBrick;


    public ObjectPool<GameObject> SpherePool;
    ObjectPool<GameObject> BrickPool;
    public int ballsInGame;

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        //if (Instance == null) 
        //{
        //    Instance = this;
        //} else if (Instance != this) 
        //{
        //    Destroy(gameObject);
        //}

        SpherePool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);
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
        }
    }

    private GameObject CreatePooledItem()
    {
        GameObject ball;

        ball = Instantiate(prefabBall, transform.position, Quaternion.identity);

        return ball;
    }

    public override void UpdateMe()
    {
        if (ballsInGame == 0)
            CreateSphere();


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
            CreateSphere();
            CreateSphere();
        }
    }

    private void CreateSphere()
    {
        GameObject Sphere = SpherePool.Get();
        ballsInGame++;
        Sphere.transform.position = transform.position;
        SphereController sphereController = Sphere.GetComponent<SphereController>();
        sphereController.SpherePool = SpherePool;
        CustomUpdateManager.Instance.scriptsBehaviour.Add(sphereController);
        sphereController.InitialLaunch = false;
    }
}
