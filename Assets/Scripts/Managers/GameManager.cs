using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;
using UnityEngine.Audio;
using Unity.Mathematics;

public class GameManager : CustomUpdateManager
{
    public static GameManager Instance;

    [Header("Gameplay Properties")]

    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;

    public List<Vector3> BrickPositions = new List<Vector3>();
    public List<BrickController> Bricks = new List<BrickController>();
    public GameObject levelObjects;
    public GameObject levelParent;
    public GameObject powerUpParent;
    public ObjectPool<GameObject> BrickPool;

    public ObjectPool<GameObject> SpherePool;
    public int ballsInGame;
    string sceneName;
    List<SphereController> sphereControllers = new List<SphereController>();
    public List<SphereController> SphereControllers { get => sphereControllers; }

    public GameObject lifeManager;
    public GameObject particleParent;
    public ParticlePool particlePool;
    public List<GameObject> brickVariations = new List<GameObject>();
    [SerializeField]
    private List<AssetReferenceGameObject> assetReferencesGameObjects;

    [SerializeField]
    private List<AssetReferenceTexture2D> assetReferencesTextures2D;

    bool initialized;
    int score = 0;
    int level = 1;
    int playerLifes = 3;
    public int ballBounce;
    public int blocksLeft;

    [Header("Audio Settings")]
    public AudioMixerGroup Global;
    public AudioMixerGroup BGM;
    public AudioMixerGroup SFX;
    public TextMeshProUGUI GlobalVolText, BGMVolText, SFXVolText;
    public int GlobalVol, BGMVol, SFXVol;

    public AudioSource SFXAudiorSource;

    public AudioClip LoseLifeClip, BallBounceClip, ExplosionClip, PowerDownClip, PowerUpClip, SelectClip, PowerUpSpawnClip, WinClip;

    [Header("Pause Settings")]
    public GameObject PauseObject;
    public List<GameObject> PauseButtonsList;
    public GameObject PointerPause;

    [Header("PowerUp Settings")]
    public PowerUpSettings PowerUpSettings;

    [Header("UI")]
    public TextMeshProUGUI ScoreCount;
    public TextMeshProUGUI LevelCount;
    public TextMeshProUGUI powerUpText;
    public TextMeshProUGUI Blocksleft;
    public TextMeshProUGUI BallBounce;

    [Header("SplashScreen")]
    [SerializeField] List<Image> texts;

    [Header("MainMenu")]
    [SerializeField] GameObject TitleObject;
    [SerializeField] GameObject ButtonSelect;

    [Header("ParallaxBackground")]
    public List<GameObject> parallaxPlane = new List<GameObject>();
    public List<float> parallaxScales = new List<float>();

    [Header("Prefabs")]
    public GameObject PlayerRect;
    public GameObject prefabBall;
    [SerializeField] List<GameObject> bricksPrefab = new List<GameObject>();
    [SerializeField] GameObject destroyParticles;
    [SerializeField] GameObject ballDestroyParticles;

    [Header("Managers")]
    public PauseManager PauseManager;
    public LevelManager LevelManager;
    public AssetsManager assetsManager;
    BallManager ballManager;
    CollisionManager collisionManager;
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

        assetsManager = new AssetsManager();
        assetsManager.Initialize();

        SpherePool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);

        ballManager = new BallManager();
        collisionManager = new CollisionManager();
        LevelManager = new LevelManager();
        PauseManager = new PauseManager();

        ballManager.GameObject = gameObject;
        collisionManager.GameObject = gameObject;
        LevelManager.GameObject = gameObject;
        PauseManager.GameObject = gameObject;

        BallMaterialBlock = new MaterialPropertyBlock();
        BallMaterialBlock.SetColor("_Color", Color.white);

        particlePool = new ParticlePool();

        GenerateBrickGrid();

        //if (SceneManager.GetActiveScene().name == "Gameplay")
        //    LevelManager.InitializeLevel();

        SetParallax();
    }

    private void OnDestroyPoolObject(GameObject Gobject)
    {
        Destroy(Gobject);
    }

    public void PlayAudio(AudioClip audioClip) 
    {
        SFXAudiorSource.clip = audioClip;
        SFXAudiorSource.Play();
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
        GameObject ball = GetInstance("sphere");
        ball.transform.parent = levelObjects.transform;
        ball.transform.position = transform.position;
        return ball;
    }

    public void InitializePool()
    {
        BrickPool = new ObjectPool<GameObject>(CreateBrickItem, BrickOnTakeFromPool, BrickOnReturnedToPool, BrickOnDestroyPoolObject, true, 28, 28);
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

    public GameObject CreateBrickVariation(BrickController controller, string name) 
    {
        GameObject brick = GetInstance(name);

        brick.transform.position = controller.GameObject.transform.position;
        brick.transform.rotation = controller.GameObject.transform.rotation;
        brick.transform.parent = levelParent.transform;

        GameObject previousBrick = controller.GameObject;

        controller.GameObject = brick;

        Destroy(previousBrick);

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

    private void SetParallax()
    {
        for(int i = 0; i < parallaxPlane.Count; i++)
        {
            GameObject plane = parallaxPlane[i];
            float scale = (i < parallaxScales.Count) ? parallaxScales[i] : 0.5f;

            ParallaxController parallax = new ParallaxController();
            parallax.GameObject = plane;
            parallax.InitializeParallax(10f, scale, Player);
            scriptsBehaviourNoMono.Add(parallax);
        }
    }

    public override void Update()
    {
        base.Update();

        if (!initialized && assetsManager.assetsLoaded)
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
                PauseLogic();
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
            Time.timeScale = 1f;
            scriptsBehaviourNoMono.Add(PauseManager);
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
        LevelManager.InitializeLevel();
        scriptsBehaviourNoMono.Add(LevelManager);
        scriptsBehaviourNoMono.Add(PauseManager);
        scriptsBehaviourNoMono.Add(Player);
        particlePool.InitializePool();

        InitializePool();
    }

    private void GameplayUpdate()
    {
        BallBounce.text = $"{ballBounce}";
        Blocksleft.text = $"{blocksLeft}";
        PauseLogic();

        UpdatePowerUpText();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LevelManager.CreateSphere();
            LevelManager.CreateSphere();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EnableFireBall();
        }
#endif
    }

    private void PauseLogic()
    {
        Global.audioMixer.GetFloat("MasterVol", out float Globalvalue);
        BGM.audioMixer.GetFloat("BGMVol", out float BGMvalue);
        SFX.audioMixer.GetFloat("BGSVol", out float BGSvalue);

        GlobalVol = (int)math.remap(-80, 20, 0, 100, Globalvalue);
        BGMVol = (int)math.remap(-80, 20, 0, 100, BGMvalue);
        SFXVol = (int)math.remap(-80, 20, 0, 100, BGSvalue);



        GlobalVolText.text = GlobalVol.ToString();
        BGMVolText.text = BGMVol.ToString();
        SFXVolText.text = SFXVol.ToString();

        if (Input.GetKeyDown(KeyCode.Escape) && sceneName == "Gameplay")
        {
            PauseTrigger();
        }
    }

    public void PauseTrigger()
    {
        for (int i = 0; i < scriptsBehaviourNoMono.Count; i++)
            scriptsBehaviourNoMono[i].isPaused = !scriptsBehaviourNoMono[i].isPaused;

        PauseManager.index = 0;

        PauseObject.SetActive(!PauseObject.activeSelf);

        Time.timeScale = PauseObject.activeSelf ? 0.00001f : 1;
    }

    public void LevelAppear()
    {
        StartCoroutine(LevelLerp());
    }

    public Color[] GetRandomGradient()
    {
        return colorGradients[Random.Range(0, colorGradients.Length)];
    }

    public void LoadAssets()
    {
        StartCoroutine(LoadAssetsCoroutine());
    }

    public GameObject GetInstance(string assetName)
    {
        if (assetsManager.loadedAssetsGameObjects.ContainsKey(assetName))
        {
            return Instantiate(assetsManager.loadedAssetsGameObjects[assetName]);
        }
        UnityEngine.Debug.LogError($"Asset '{assetName}' not found.");
        return null;
    }

    #region PowerUps Functions

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
    public void MultipleBallEffect()
    {
        for (int i = 0; i < PowerUpSettings.BallsToSpawn; i++)
        {
            LevelManager.CreateSphere();
        }
    }

    public void EnableFireBall()
    {
        StartCoroutine(FireBallEffect());
    }
    public void ChangeSizePlayerEffect(float multiplier, PowerUpController powerUp)
    {
        if (!onPowerUpMode)
            StartCoroutine(PlayerLong(multiplier, powerUp));
    }
    public void ApplyScoreMultiplier(PowerUpController powerUp)
    {
        StartCoroutine(ScoreMultiplierBuff(PowerUpSettings.scoreMultiplier, PowerUpSettings.duration, powerUp));
    }
    public void IncreaseScore(int amount)
    {
        int finalScore = Mathf.RoundToInt(amount * scoreMultiplier);
        score += finalScore;
        ScoreCount.text = score.ToString();
    }

    public void ApplySpeedBoostBuff()
    {
        StartCoroutine(SpeedBoostBuff(PowerUpSettings.speedIncreaseAmount, PowerUpSettings.SpeedDuration));
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
    #endregion

    #region Coroutines
    private IEnumerator LoadAssetsCoroutine()
    {
        int assetsToLoad = assetReferencesGameObjects.Count;
        int assetsLoaded = 0;
        bool gameObjectLoaded = false;
        foreach (AssetReferenceGameObject assetReference in assetReferencesGameObjects)
        {
            AsyncOperationHandle<GameObject> handle =
            assetReference.LoadAssetAsync<GameObject>();

            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                String assetName = handle.Result.name.Split(" ")[0];
                Debug.Log(assetName);
                assetsManager.loadedAssetsGameObjects.Add(assetName, handle.Result);
                assetsLoaded++;
            }            

        }
        if (assetsLoaded == assetsToLoad)
            gameObjectLoaded = true;

        assetsToLoad = assetReferencesTextures2D.Count;
        assetsLoaded = 0;

        foreach (AssetReferenceTexture2D assetReference in assetReferencesTextures2D)
        {
            if (assetReference.GetType() == typeof(AssetReferenceTexture2D))
            {
                AsyncOperationHandle<Texture2D> handle =
                assetReference.LoadAssetAsync<Texture2D>();

                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    String assetName = handle.Result.name.Split(" ")[0];
                    Debug.Log(assetName);

                    assetsManager.loadedAssetsTextures.Add(assetName, handle.Result);
                    assetsLoaded++;
                }
            }
        }

        if (gameObjectLoaded) 
        {
            assetsManager.ExecuteEvent();
            assetsManager.assetsLoaded = true;
        }
    }

    public IEnumerator LevelLerp()
    {
        float elapsedTime = 0;
        float lerpDuration = 1.75f;
        Vector3 initialPosition = levelParent.transform.position;
        Vector3 endPosition = Vector3.zero;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += PauseManager.isPaused ? 0 : Time.deltaTime;
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
            elapsed += PauseManager.isPaused ? 0 : Time.deltaTime;

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
        bool replaced = AddOrRefreshPowerUp(fireBallPowerUp, PowerUpSettings.fireballDuration);

        Player.fireBallPad = true;
        float elapsedTime = 0;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            Player.fireBallPad = true;

            elapsedTime += PauseManager.isPaused ? 0 : Time.deltaTime;

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
            timer -= PauseManager.isPaused ? 0 : Time.deltaTime;

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
            elapsed += PauseManager.isPaused ? 0 : Time.deltaTime;
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
            elapsedTime += PauseManager.isPaused ? 0 : Time.deltaTime;

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
    #endregion
}
