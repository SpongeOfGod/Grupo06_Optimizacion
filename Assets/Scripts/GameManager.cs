using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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
    public GameObject powerUpParent;
    public ObjectPool<GameObject> BrickPool;

    public ObjectPool<GameObject> SpherePool;
    public int ballsInGame;
    string sceneName;
    List<SphereController> sphereControllers = new List<SphereController>();
    public List<SphereController> SphereControllers { get => sphereControllers; }
    public TextMeshProUGUI ScoreCount;
    public TextMeshProUGUI LevelCount;
    public TextMeshProUGUI powerUpText;
    public GameObject lifeManager;
    public GameObject particleParent;
    public ParticlePool particlePool;

    bool initialized;
    int score = 0;
    int level = 1;
    int playerLifes = 3;

    [Header("SplashScreen")]
    [SerializeField] List<Image> texts;

    [Header("MainMenu")]
    [SerializeField] GameObject TitleObject;
    [SerializeField] GameObject ButtonSelect;

    [Header("Prefabs")]
    public GameObject PlayerRect;
    public GameObject prefabBall;
    [SerializeField] List<GameObject> bricksPrefab = new List<GameObject>();
    [SerializeField] GameObject destroyParticles;
    [SerializeField] GameObject ballDestroyParticles;

    [Header("Managers")]
    BallManager ballManager;
    CollisionManager collisionManager;
    LevelManager levelManager;
    [SerializeField] List<GameObject> powerUpControllers;

    public List<GameObject> BricksPrefab { get => bricksPrefab; }
    public List<PowerUpController> activePowerUps = new List<PowerUpController>();
    public MaterialPropertyBlock BallMaterialBlock;
    public bool onPowerUpMode;

    public List<(PowerUpController powerUp, float duration)> currentPowerUps = new List<(PowerUpController, float)>();
    public bool runningCorrutine;
    public float scoreMultiplier = 1f;

    private readonly Color[][] colorGradients = new Color[][]
    {
        new Color[] { new Color(0.5f, 0f, 0.5f), new Color(0f, 0f, 1f) },
        new Color[] { new Color(1f, 0f, 0f), new Color(0.5f, 0f, 1f) },
        new Color[] { new Color(1f, 1f, 0f), new Color(1f, 0f, 0f) },
        new Color[] { new Color(0f, 1f, 0f), new Color(0f, 0f, 1f) },
        new Color[] { new Color(0f, 0.5f, 0.5f), new Color(0f, 1f, 0f) },
    };

    private Dictionary<string, string> powerUpNameMap = new Dictionary<string, string>()
    {
        { "LongPlayerPowerUp", "Enlarged++" },
        { "ShortPlayerPowerDown", "Shrunken--" },
        { "SpeedPowerUp", "Speed++" },
        { "FireBallPowerUp", "Fireball" },
        { "ScoreMultiplierPowerUp", "Score X2" }
    };

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

        particlePool = new ParticlePool();

        GenerateBrickGrid();

        if (SceneManager.GetActiveScene().name == "Gameplay")
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

    public void InitializePool()
    {
        BrickPool = new ObjectPool<GameObject>(CreateBrickItem, BrickOnTakeFromPool, BrickOnReturnedToPool, BrickOnDestroyPoolObject, true, 30, 30);
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

    public void GenerateBrickGrid()
    {
        BrickPositions.Clear();

        int rows = 4;
        int columns = 7;
        float startX = -6.64f;
        float startY = 3.75f;
        float xSpacing = 1.14f;
        float ySpacing = 0.6f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = startX + col * xSpacing;
                float y = startY - row * ySpacing;
                Vector3 pos = new Vector3(x, y, 0f);
                BrickPositions.Add(pos);
            }
        }
    }

    private GameObject CreateBrickItem()
    {
        int index = Random.Range(0, bricksPrefab.Count);
        GameObject brick = Instantiate(bricksPrefab[index], levelParent.transform);

        return brick;
    }

    public GameObject SpawnDestroyParticles()
    {
        GameObject particles = Instantiate(destroyParticles, Vector3.zero, Quaternion.identity, particleParent.transform);

        return particles;
    }

    public void GiveColorParticle(GameObject particles, Color color)
    {
        var renderer = particles.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", color);
            renderer.SetPropertyBlock(propertyBlock);
        }

        var system = particles.GetComponent<ParticleSystem>();
        if (system != null)
        {
            system.Play();
        }
    }

    public void SpawnBallDeathParticles(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        GameObject particles = Instantiate(ballDestroyParticles, new Vector3 (position.x, -5.69f, 0), rotation);

        var renderer = particles.GetComponent<ParticleSystemRenderer>();

        var system = particles.GetComponent<ParticleSystem>();
        if (system != null)
        {
            system.Play();
        }
    }

    public void IncreaseLevel()
    {
        level++;
        LevelCount.text = level.ToString();
    }

    public void PlayerLifesChanges()
    {
        if (playerLifes - 1 >= 0)
        {
            lifeManager.transform.GetChild(playerLifes - 1).gameObject.SetActive(false);
            playerLifes--;
        }

        if (playerLifes <= 0)
            SceneManager.LoadScene("Gameplay");
    }

    public PowerUpController CreatePowerUp(Vector3 position)
    {
        int index = Random.Range(0, powerUpControllers.Count);
        GameObject powerUp = Instantiate(powerUpControllers[index], levelParent.transform);
        PowerUpController powerUpController = null;
        Renderer renderer = powerUp.GetComponent<Renderer>();

        switch (index)
        {
            case 0:
                powerUpController = new MultiBallPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.green);
                break;
            case 1:
                powerUpController = new LongPlayerPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.blue);
                break;
            case 2:
                powerUpController = new ShortPlayerPowerDown();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.red);
                break;
            case 3:
                powerUpController = new FireBallPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.yellow);
                break;
            case 4:
                powerUpController = new SpeedPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.magenta);
                break;
            case 5:
                powerUpController = new BombPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.cyan);
                break;

            case 6:
                powerUpController = new ScoreMultiplierPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.white);
                break;
        }

        renderer.SetPropertyBlock(BallMaterialBlock);
        powerUpController.GameObject.transform.position = position - new Vector3(0, 0, 0.03f);
        scriptsBehaviourNoMono.Add(powerUpController);
        activePowerUps.Add(powerUpController);
        //powerUpController.GameObject.SetActive(false);

        return powerUpController;
    }

    public void DestroyPowerUp(PowerUpController powerUp)
    {
        scriptsBehaviourNoMono.Remove(powerUp);
        activePowerUps.Remove(powerUp);
        Destroy(powerUp.GameObject);
        powerUp = null;
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
                    InitializeSplashScreen();
                    break;

                case "MainMenu":
                    InitializeMainMenu();
                    break;

                case "Gameplay":
                    InitializeGameplay();
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

    private void InitializeSplashScreen()
    {
        List<SplashText> list = new List<SplashText>();
        foreach (var item in texts)
        {
            SplashText text = new SplashText { ImageColor = item };
            list.Add(text);
            scriptsBehaviourNoMono.Add(text);
        }

        SplashController splashController = new SplashController
        {
            splashTexts = list,
            GameObject = gameObject
        };
        scriptsBehaviourNoMono.Add(splashController);
        splashController.Initialize();
    }

    private void InitializeMainMenu()
    {
        if (TitleObject != null)
        {
            TitleAnimation titleAnimation = new TitleAnimation { GameObject = TitleObject };
            scriptsBehaviourNoMono.Add(titleAnimation);

            ButtonSelector buttonSelector = new ButtonSelector { GameObject = ButtonSelect };
            scriptsBehaviourNoMono.Add(buttonSelector);
        }
    }

    private void InitializeGameplay()
    {
        scriptsBehaviourNoMono.Add(ballManager);
        scriptsBehaviourNoMono.Add(collisionManager);
        scriptsBehaviourNoMono.Add(levelManager);
        scriptsBehaviourNoMono.Add(Player);
        particlePool.InitializePool();

        InitializePool();
    }



    private void GameplayUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");


        UpdatePowerUpText();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            levelManager.CreateSphere();
            levelManager.CreateSphere();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EnableFireBall();
        }
#endif
    }

    public void MultipleBallEffect()
    {
        levelManager.CreateSphere();
        levelManager.CreateSphere();
    }

    public void ChangeSizePlayerEffect(float SizeMultiplier, PowerUpController powerUp)
    {
        if (!onPowerUpMode)
            StartCoroutine(PlayerLong(SizeMultiplier, powerUp));
    }

    public void LevelAppear()
    {
        StartCoroutine(LevelLerp());
    }

    public Color[] GetRandomGradient()
    {
        return colorGradients[Random.Range(0, colorGradients.Length)];
    }

    public void EnableFireBall()
    {
        StartCoroutine(FireBallEffect());
    }

    public void ApplyScoreMultiplier(float amount, float duration, PowerUpController powerUp)
    {
        StartCoroutine(ScoreMultiplierBuff(amount, duration, powerUp));
    }
    public void IncreaseScore(int amount)
    {
        int finalScore = Mathf.RoundToInt(amount * scoreMultiplier);
        score += finalScore;
        ScoreCount.text = score.ToString();
    }

    public void ApplySpeedBoostBuff(float amount, float duration)
    {
        StartCoroutine(SpeedBoostBuff(amount, duration));
    }

    public bool AddOrRefreshPowerUp(PowerUpController newPowerUp, float newDuration)
    {
        bool replaced = false;

        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            if (currentPowerUps[i].powerUp.GetType() == newPowerUp.GetType())
            {
                currentPowerUps[i] = (newPowerUp, newDuration);
                replaced = true;
                return true;
            }
        }

        if (!replaced)
        {
            currentPowerUps.Add((newPowerUp, newDuration));
            return false;
        }

        return false;
    }

    private void UpdatePowerUpText()
    {
        if (currentPowerUps.Count == 0)
        {
            if (powerUpText.text != "")
                powerUpText.text = "";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var (powerUp, powerUpDuration) in currentPowerUps)
        {
            string className = powerUp.GetType().Name;

            string displayName = powerUpNameMap.TryGetValue(className, out string name)
                ? name
                : className;

            sb.AppendLine($"<color=white>{displayName} ({powerUpDuration:F1}s)</color>");
        }

        powerUpText.text = sb.ToString();
    }

    public IEnumerator LevelLerp()
    {
        float elapsedTime = 0;
        float lerpDuration = 1.75f;
        Vector3 initialPosition = levelParent.transform.position;
        Vector3 endPosition = Vector3.zero;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = 1 - Mathf.Pow(1 - (elapsedTime / lerpDuration), 3);
            levelParent.transform.position = Vector3.Lerp(initialPosition, endPosition, lerpFactor);
            yield return null;
        }

        levelParent.transform.position = endPosition;
    }

    IEnumerator ScoreMultiplierBuff(float multiplier, float duration, PowerUpController powerUp)
    {
        scoreMultiplier = multiplier;

        Color colorA = Color.red;
        Color colorB = Color.yellow;
        float flickerSpeed = 5f;

        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.PingPong(Time.time * flickerSpeed, 1f);
            ScoreCount.color = Color.Lerp(colorA, colorB, t);

            for (int i = 0; i < currentPowerUps.Count; i++)
            {
                PowerUpController powerUpI = currentPowerUps[i].powerUp;

                if (powerUpI == powerUp)
                    currentPowerUps[i] = (powerUp, duration - elapsed);
            }

            yield return null;
        }

        scoreMultiplier = 1f;
        ScoreCount.color = Color.white;

        currentPowerUps.RemoveAll(x => x.powerUp == powerUp);
    }


    IEnumerator FireBallEffect()
    {
        PowerUpController fireBallPowerUp = new FireBallPowerUp();
        bool replaced = AddOrRefreshPowerUp(fireBallPowerUp, 5f);

        Player.fireBallPad = true;
        float elapsedTime = 0;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            Player.fireBallPad = true;

            elapsedTime += Time.deltaTime;

            for (int i = 0; i < currentPowerUps.Count; i++)
            {
                PowerUpController powerUpI = currentPowerUps[i].powerUp;

                if(powerUpI == fireBallPowerUp)
                    currentPowerUps[i] = (fireBallPowerUp, duration - elapsedTime);
            }

            yield return null;
        }

        Player.fireBallPad = false;

        currentPowerUps.RemoveAll(x => x.powerUp == fireBallPowerUp);
    }

    IEnumerator PlayerLong(float sizeMultiplier, PowerUpController powerUp)
    {
        float transitionDuration = 0.33f;
        float holdDuration = 5f;
        float totalDuration = transitionDuration * 2 + holdDuration;

        AddOrRefreshPowerUp(powerUp, totalDuration);

        onPowerUpMode = true;

        Vector3 initialSize = Player.Size;
        Vector3 enlargedSize = new Vector3(initialSize.x * sizeMultiplier, initialSize.y, initialSize.z);

        Vector3 initialScale = PlayerRect.transform.localScale;
        Vector3 enlargedScale = new Vector3(initialScale.x, (enlargedSize.x * initialScale.y) / initialSize.x, initialScale.z);

        yield return ScaleOverTime(initialSize, enlargedSize, initialScale, enlargedScale, transitionDuration);

        float timer = holdDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            for (int i = 0; i < currentPowerUps.Count; i++)
            {
                PowerUpController powerUpI = currentPowerUps[i].powerUp;

                if (powerUp == powerUpI)
                    currentPowerUps[i] = (powerUp, timer);
            }

            yield return null;
        }

        yield return ScaleOverTime(enlargedSize, initialSize, enlargedScale, initialScale, transitionDuration);

        Player.Size = initialSize;
        PlayerRect.transform.localScale = initialScale;

        onPowerUpMode = false;

        currentPowerUps.RemoveAll(x => x.powerUp == powerUp);
    }

    IEnumerator ScaleOverTime(Vector3 fromSize, Vector3 toSize, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Player.Size = Vector3.Lerp(fromSize, toSize, t);
            PlayerRect.transform.localScale = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }

        Player.Size = toSize;
        PlayerRect.transform.localScale = toScale;
    }

    IEnumerator SpeedBoostBuff(float amount, float duration)
    {
        PowerUpController speedBoostPowerUp = new SpeedPowerUp();
        AddOrRefreshPowerUp(speedBoostPowerUp, duration);

        foreach (var ball in SphereControllers)
        {
            if (ball != null)
                ball.IncreaseSpeed(amount);
        }

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < currentPowerUps.Count; i++)
            {
                var powerUp = currentPowerUps[i];

                if (powerUp.powerUp == speedBoostPowerUp)
                    currentPowerUps[i] = (powerUp.powerUp, powerUp.duration - Time.deltaTime);
            }

            yield return null;
        }

        foreach (var ball in SphereControllers)
        {
            if (ball != null)
                ball.ResetSpeed();
        }

        currentPowerUps.RemoveAll(x => x.powerUp == speedBoostPowerUp);
    }
}
