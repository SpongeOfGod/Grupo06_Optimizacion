using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : ManagedUpdateBehaviour
{
    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;
    public List<Vector3> BrickPositions = new List<Vector3>();
    public List<MaterialPropertyBlock> BrickMaterials = new List<MaterialPropertyBlock>();
    public Dictionary<Vector2, GameObject> grid = new Dictionary<Vector2, GameObject>(); 
    Dictionary<GameObject, Renderer> bricksMaterial = new Dictionary<GameObject, Renderer>();
    public List<BrickController> Bricks = new List<BrickController>();


    private int numberOfBricks;
    public ObjectPool<GameObject> SpherePool;
    public BrickPool brickPool;


    public int ballsInGame;
    bool initialized;

    public static GameManager Instance;

    [Header("Prefabs")]

    public GameObject PlayerRect;
    public GameObject prefabBall;
    public GameObject prefabBrick;

    [Header("Managers")]

    [SerializeField] BallManager ballManager;

    List<SphereController> sphereControllers = new List<SphereController>();
    public List<SphereController> SphereControllers { get => sphereControllers; }

    private void Awake()
    {
        Instance = this;

        brickPool = GetComponent<BrickPool>();

        SpherePool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);

        ballManager = new BallManager();

        for (int i = 0; i < BrickPositions.Count; i++)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            brickPool.InitializePool();
            GameObject brick = brickPool.Pool.Get();
            brick.transform.position = BrickPositions[i];
            BrickController brickController = brick.GetComponent<BrickController>();

            Renderer renderer = brick.GetComponent<Renderer>();
            Bricks.Add(brickController);

            bricksMaterial.TryAdd(brick, renderer);
            numberOfBricks++;

            SetPositionAndColor(brick, BrickPositions[i]);
        }
    }

    private void OnDestroyPoolObject(GameObject Gobject)
    {
        Destroy(Gobject);
    }

    private void SetPositionAndColor(GameObject brick, Vector3 position) 
    {
        if (!grid.ContainsKey(position)) 
        {
            brick.transform.position = position;
            grid.Add(position, brick);
        }

        bricksMaterial.TryGetValue(brick, out Renderer renderer);

        if (numberOfBricks < 8) 
        {
            MaterialPropertyBlock material = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(material);
            material.SetColor("_Color", Color.red);
            renderer.SetPropertyBlock(material);
        }
        else if (numberOfBricks >= 8 && numberOfBricks < 15)
        {
            MaterialPropertyBlock material = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(material);
            material.SetColor("_Color", Color.blue);
            renderer.SetPropertyBlock(material);
        }
        else if (numberOfBricks >= 15 && numberOfBricks < 22)
        {
            MaterialPropertyBlock material = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(material);
            material.SetColor("_Color", Color.green);
            renderer.SetPropertyBlock(material);
        }
        else if (numberOfBricks >= 22 && numberOfBricks < 29)
        {
            MaterialPropertyBlock material = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(material);
            material.SetColor("_Color", Color.yellow);
            renderer.SetPropertyBlock(material);
        }
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
        if (!initialized) 
        {
            CustomUpdateManager.Instance.scriptsBehaviourNoMono.Add(ballManager);
        }

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
                //MeshRenderer meshRenderer = item.gameObject.GetComponent<MeshRenderer>();
                //MaterialPropertyBlock Material = new MaterialPropertyBlock();
                //meshRenderer.GetPropertyBlock(Material);
                //Material.SetColor("_Color", Color.red);
                numberOfBricks = 0;
                //meshRenderer.SetPropertyBlock(Material);
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
        SphereController sphereController = new SphereController();
        sphereController.sphere = Sphere;
        sphereController.SpherePool = SpherePool;
        ballsInGame++;
        //Sphere.transform.position = transform.position;
        sphereControllers.Add(sphereController);
        sphereController.SpherePool = SpherePool;
        CustomUpdateManager.Instance.scriptsBehaviourNoMono.Add(sphereController);
        sphereController.InitialLaunch = false;
    }
}
