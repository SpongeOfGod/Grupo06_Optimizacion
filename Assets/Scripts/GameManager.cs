using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : CustomUpdateManager
{
    public static GameManager Instance;

    [Header("Gameplay Properties")]
    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;

    public List<Vector3> BrickPositions = new List<Vector3>();
    public List<BrickController> Bricks = new List<BrickController>();
    public GameObject levelParent;
    public ObjectPool<GameObject> BrickPool;

    public ObjectPool<GameObject> SpherePool;
    public int ballsInGame;
    string sceneName;
    List<SphereController> sphereControllers = new List<SphereController>();
    public List<SphereController> SphereControllers { get => sphereControllers; }

    bool initialized;

    [Header("SplashScreen")]
    [SerializeField] List<Image> texts;

    [Header("MainMenu")]
    [SerializeField] GameObject TitleObject;
    [SerializeField] GameObject ButtonSelect;

    [Header("Prefabs")]

    public GameObject PlayerRect;
    public GameObject prefabBall;
    [SerializeField] List<GameObject> bricksPrefab = new List<GameObject>();

    [Header("Managers")]

    BallManager ballManager;
    CollisionManager collisionManager;
    LevelManager levelManager;

    [SerializeField] GameObject[] powerUpControllers;
    public List<PowerUpController> activePowerUps = new List<PowerUpController>();

    public MaterialPropertyBlock BallMaterialBlock;


    public void InitializePool()
    {
        BrickPool = new ObjectPool<GameObject>(CreateBrickItem, BrickOnTakeFromPool, BrickOnReturnedToPool, BrickOnDestroyPoolObject, true, 50, 50);
    }
    private void BrickOnDestroyPoolObject(GameObject Gobject)
    {
        Destroy(Gobject);
    }

    private void BrickOnReturnedToPool(GameObject Gobject)
    {
        Gobject.SetActive(false);
    }

    private void BrickOnTakeFromPool(GameObject Gobject)
    {
        if (Gobject != null)
        {
            Gobject.SetActive(true);
        }
    }

    private GameObject CreateBrickItem()
    {
        int index = Random.Range(0, bricksPrefab.Count);

        GameObject brick = Instantiate(bricksPrefab[index], levelParent.transform);

        return brick;
    }

    public GameObject CreatePowerUp() 
    {
        int index = Random.Range(0, powerUpControllers.Length - 1);

        GameObject powerUp = Instantiate(powerUpControllers[index], levelParent.transform);

        PowerUpController powerUpController = new MultiBallPowerUp();

        powerUpController.GameObject = powerUp;

        scriptsBehaviourNoMono.Add(powerUpController);

        activePowerUps.Add(powerUpController);

        return powerUpController.GameObject;
    }

    public void DestroyPowerUp(PowerUpController powerUp) 
    {
        scriptsBehaviourNoMono.Remove(powerUp);
        activePowerUps.Remove(powerUp);
        Destroy(powerUp.GameObject);
    }

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        Instance = this;

        Player = new PlayerMovement();

        Player.GameObject = PlayerRect;

        SpherePool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);

        ballManager = new BallManager();
        collisionManager = new CollisionManager();
        levelManager = new LevelManager();

        ballManager.GameObject = gameObject;
        collisionManager.GameObject = gameObject;
        levelManager.GameObject = gameObject;

        BallMaterialBlock = new MaterialPropertyBlock();
        BallMaterialBlock.SetColor("_Color", Color.white);

        levelManager.InitializeLevel();
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

    public override void Update()
    {
        base.Update();

        if (!initialized)
        {
            initialized = true;

            switch (sceneName)
            {
                case "SplashScreen":
                    List<SplashText> list = new List<SplashText>();
                    foreach (var item in texts)
                    {
                        SplashText text = new SplashText();
                        text.ImageColor = item;
                        list.Add(text);

                        scriptsBehaviourNoMono.Add(text);
                    }

                    SplashController splashController = new SplashController();
                    splashController.splashTexts = list;
                    splashController.GameObject = gameObject;
                    scriptsBehaviourNoMono.Add(splashController);
                    splashController.Initialize();
                    break;

                case "MainMenu":
                    if (TitleObject != null)
                    {
                        TitleAnimation titleAnimation = new TitleAnimation();
                        titleAnimation.GameObject = TitleObject;
                        scriptsBehaviourNoMono.Add(titleAnimation);

                        ButtonSelector buttonSelector = new ButtonSelector();
                        buttonSelector.GameObject = ButtonSelect;
                        scriptsBehaviourNoMono.Add(buttonSelector);
                    }

                    break;

                case "Gameplay":
                    scriptsBehaviourNoMono.Add(ballManager);
                    scriptsBehaviourNoMono.Add(collisionManager);
                    scriptsBehaviourNoMono.Add(levelManager);
                    scriptsBehaviourNoMono.Add(Player);
                    break;
            }
        }

        switch (sceneName)
        {
            case "MainMenu":
                break;

            case "Gameplay":
                GameplayUpdate();
                break;
        }
    }

    private void GameplayUpdate()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            levelManager.CreateSphere();
            levelManager.CreateSphere();
        }
    #endif
    }

    public void MultipleBallEffect() 
    {
        levelManager.CreateSphere();
        levelManager.CreateSphere();
    }
}
