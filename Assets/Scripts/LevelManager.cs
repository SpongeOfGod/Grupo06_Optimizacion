using System.Collections.Generic;
using UnityEngine;
public class LevelManager : ManagedUpdateBehaviourNoMono
{
    private Dictionary<Vector2, GameObject> grid = new Dictionary<Vector2, GameObject>();
    private Dictionary<GameObject, Renderer> bricksMaterial = new Dictionary<GameObject, Renderer>();
    private Dictionary<GameObject, BrickController> brickToController = new Dictionary<GameObject, BrickController>();

    public List<BrickController> Bricks = new List<BrickController>();
    public Dictionary<GameObject, Renderer> BricksMaterial { get => bricksMaterial; }

    GameManager gManager;
    int powerUpCount = 0;

    int internalLevel = 0;

    public void InitializeLevel()
    {
        if (!gManager)
            gManager = GameManager.Instance;

        if (!GameManager.Instance.levelParent.gameObject)
            GameManager.Instance.levelParent.gameObject.SetActive(false);

        Color[] selectedGradient = gManager.GetRandomGradient();

        
        gManager.InitializePool();

        for (int i = 0; i < gManager.BrickPositions.Count; i++)
        {
            GameObject brick = gManager.BrickPool.Get();
            brick.transform.position = gManager.BrickPositions[i];

            BrickController brickController = new BrickController();
            brickController.MyPool = gManager.BrickPool;
            brickController.GameObject = brick;

            PowerUpController powerUpController = null;
            if (powerUpCount < gManager.PowerUpSettings.QuantityPerLevel && Random.value > 0.70f)
            {
                powerUpController = gManager.CreatePowerUp(brickController.GameObject.transform.position);
                powerUpCount++;
            }

            if (powerUpController != null) 
            {
                brickController.powerUp = powerUpController;
                Renderer rendererPowerUp = powerUpController.GameObject.GetComponent<Renderer>();
                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                rendererPowerUp.GetPropertyBlock(materialPropertyBlock);
                brickController.PowerUpColor = materialPropertyBlock.GetColor("_Color");
                materialPropertyBlock.SetColor("_Color", Color.black);
                rendererPowerUp.SetPropertyBlock(materialPropertyBlock);
            }

            Renderer renderer = brick.GetComponent<Renderer>();
            gManager.Bricks.Add(brickController);
            Bricks.Add(brickController);

            bricksMaterial.TryAdd(brick, renderer);
            brickToController.TryAdd(brick, brickController);

            SetPositionAndColor(brickController, gManager.BrickPositions[i], selectedGradient);
        }

        foreach (var brick in gManager.Bricks)
            GameManager.Instance.blocksLeft++;

        GameManager.Instance.levelParent.transform.position = new Vector3(0, 4, 0);
        GameManager.Instance.levelParent.gameObject.SetActive(true);
        GameManager.Instance.LevelAppear();
    }

    public override void UpdateMe()
    {
        if (!gManager.assetsManager.assetsLoaded) return;

        if (gManager.ballsInGame == 0)
            CreateSphere();

        bool anyBrickActive = false;

        //for (int i = gManager.Bricks.Count; i < 0; i--)
        //{
        //    if (gManager.Bricks[i].GameObject == null)
        //        gManager.Bricks.RemoveAt(i);
        //}
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Period)) 
        {
            internalLevel++;
            foreach (var brick in Bricks)
            {
                while (brick.Durability > 0) 
                {
                    brick.CollideReaction();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            internalLevel--;
            internalLevel--;

            foreach (var brick in Bricks)
            {
                while (brick.Durability > 0)
                {
                    brick.CollideReaction();
                }
            }
        }

#endif

        var bricksNumber = 0;
        foreach (var item in gManager.Bricks)
        {
            if (item.GameObject != null && item.GameObject.activeSelf) 
            {
                bricksNumber++;
                anyBrickActive = true;
            }
        }
        GameManager.Instance.blocksLeft = bricksNumber;

        if (!anyBrickActive)
        {
            internalLevel++;

            if (internalLevel >= 11 || internalLevel <= 0) 
            {
                internalLevel = 1;
            }
            Color[] selectedGradient = gManager.GetRandomGradient();
            powerUpCount = 0;
            GameManager.Instance.IncreaseLevel();
            GameManager.Instance.levelParent.transform.position = new Vector3(0, 4, 0);

            for (int i = 0; i < gManager.BrickPositions.Count; i++)
            {
                var item = gManager.Bricks[i];
                item.Durability = 1;

                if (LevelCreationLogic(i)) continue;

                GameObject brick = gManager.BrickPool.Get();
                item.GameObject = brick;
                LevelBrickDurability(i, item);

                item.GameObject.transform.localPosition = gManager.BrickPositions[i];

                item.GameObject.SetActive(true);

                PowerUpController powerUpController = null;
                if (powerUpCount < gManager.PowerUpSettings.QuantityPerLevel && Random.value > 0.70f)
                {
                    powerUpCount++;
                    powerUpController = gManager.CreatePowerUp(item.GameObject.transform.position);
                }

                if (powerUpController != null)
                {
                    item.powerUp = powerUpController;
                    Renderer renderer = powerUpController.GameObject.GetComponent<Renderer>();
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(materialPropertyBlock);
                    item.PowerUpColor = materialPropertyBlock.GetColor("_Color");
                    materialPropertyBlock.SetColor("_Color", Color.black);
                    renderer.SetPropertyBlock(materialPropertyBlock);
                }

                Renderer rendererBrick = item.GameObject.GetComponent<Renderer>();

                bricksMaterial.TryAdd(item.GameObject, rendererBrick);
                brickToController.TryAdd(item.GameObject, item);

                SetPositionAndColor(item, item.GameObject.transform.localPosition, selectedGradient);
            }
            GameManager.Instance.LevelAppear();
        }
    }

    public bool LevelCreationLogic(int indexToSearch) 
    {
        List<int> BricksToNotActivate = null;
        switch (internalLevel)
        {
            case 1:
                BricksToNotActivate = new List<int> { 0, 6, 9, 11, 21, 23, 25, 27 };

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 2:
                BricksToNotActivate = new List<int> { 0, 2, 4, 6, 22, 24, 26};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 3:
                BricksToNotActivate = new List<int> { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 4:
                BricksToNotActivate = new List<int> { 1, 5, 7, 9, 11, 13, 15, 19, 22, 24, 26};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 5:
                BricksToNotActivate = new List<int> { 1, 4, 5, 6, 8, 12, 15, 19, 21, 22, 23, 26};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 6:
                BricksToNotActivate = new List<int> { 2, 3, 4, 14, 15, 16, 18, 19, 20 };

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 7:
                BricksToNotActivate = new List<int> { 8, 10, 12, 15, 17, 19 };

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 9:
                BricksToNotActivate = new List<int> { 2, 3, 4, 7, 8, 9, 11, 12, 13, 16, 18};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;

            case 10:
                BricksToNotActivate = new List<int> { 1, 5, 8, 9, 11, 12, 22, 26};

                if (BricksToNotActivate.Contains(indexToSearch))
                    return true;
                break;
        }
        return false;
    }

    public void LevelBrickDurability(int indexToSearch, BrickController controller)
    {
        List<int> Durability_2 = null;
        List<int> Durability_3 = null;
        List<int> Durability_4 = null;

        switch (internalLevel)
        {
            case 1:
                Durability_2 = new List<int> { 1, 2, 3, 4, 5, 7, 13, 14, 20 };

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);
                break;

            case 2:
                Durability_2 = new List<int> { 1, 3, 5, 21, 23, 25, 27};

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_3 = new List<int> { 7, 9, 11, 13, 15, 17, 19};

                ChangeDurability(indexToSearch, controller, Durability_3, 3, 2);
                break;

            case 3:
                Durability_4 = new List<int> {0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 4:
                Durability_2 = new List<int> { 0, 2, 3, 4, 6, 10, 17 };

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_4 = new List<int> { 8, 12 };

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 5:
                Durability_2 = new List<int> { 2, 3, 9, 10, 11, 16, 17, 18, 24, 25};

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_4 = new List<int> { 0, 7, 13, 14, 20, 27};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 6:
                Durability_2 = new List<int> { 7, 8, 12, 13, 17, 21, 22, 23, 24, 25, 26, 27};

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_4 = new List<int> { 0, 1, 5, 6, 9, 10, 11};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 7:
                Durability_2 = new List<int> { 14, 16, 18, 20};

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_3 = new List<int> { 0, 2, 4, 6};

                ChangeDurability(indexToSearch, controller, Durability_3, 3, 2);

                Durability_4 = new List<int> {7, 9, 11, 13, 21, 22, 23, 24, 25, 26, 27};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 8:
                Durability_2 = new List<int> { 7, 9, 10, 11, 13, 14, 17, 20, 21, 23, 25, 27 };

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_3 = new List<int> {1, 2, 3, 4, 5, 15, 19, 22, 26};

                ChangeDurability(indexToSearch, controller, Durability_3, 3, 2);

                Durability_4 = new List<int> { 0, 6, 8, 12, 16, 18, 24};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 9:
                Durability_2 = new List<int> { 0, 1, 5, 6};

                ChangeDurability(indexToSearch, controller, Durability_2, 2, 1);

                Durability_4 = new List<int> { 10, 14, 15, 17, 19, 20, 21, 22, 23, 24, 25, 26, 27};

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;

            case 10:
                Durability_3 = new List<int> { 0, 2, 3, 4, 6, 7, 10, 13 };

                ChangeDurability(indexToSearch, controller, Durability_3, 3, 2);

                Durability_4 = new List<int> { 14, 15, 16, 17, 18, 19, 20, 21, 23, 24, 25, 27 };

                ChangeDurability(indexToSearch, controller, Durability_4, 4, 3);
                break;
        }
    }

    private static void ChangeDurability(int indexToSearch, BrickController controller, List<int> DurabilityList, int newDurability, int brickvariation)
    {
        if (DurabilityList.Contains(indexToSearch))
        {
            controller.Durability = newDurability;
            controller.GameObject = GameManager.Instance.CreateBrickVariation(controller, brickvariation);
        }
    }

    private void SetPositionAndColor(BrickController brick, Vector3 position, Color[] gradient)
    {
        if (!grid.ContainsKey(position))
        {
            brick.GameObject.transform.position = position;
            grid.Add(position, brick.GameObject);
        }

        bricksMaterial.TryGetValue(brick.GameObject, out Renderer renderer);

        float minY = 1.9f;
        float maxY = 3.75f;
        float t = Mathf.InverseLerp(minY, maxY, position.y);

        Color interpolatedColor = Color.Lerp(gradient[0], gradient[1], t);

        if (renderer != null) 
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            brick.MaterialPropertyBlock = propertyBlock;
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", interpolatedColor);
            renderer.SetPropertyBlock(propertyBlock);
        }

        if (brickToController.TryGetValue(brick.GameObject, out var controller))
        {
            controller.brickColor = interpolatedColor;
        }
    }

    public void CreateSphere()
    {
        GameObject Sphere = gManager.SpherePool.Get();
        bool SphereExisted = false;
        SphereController sphereController = new SphereController();
        sphereController.trailRenderer = Sphere.GetComponent<TrailRenderer>();

        for (int i = 0; i < gManager.SphereControllers.Count; i++)
        {
            if (gManager.SphereControllers[i] != null && gManager.SphereControllers[i].GameObject == Sphere)
            {
                SphereExisted = true;
                sphereController = gManager.SphereControllers[i];
            }
        }

        if (!SphereExisted)
        {
            sphereController.GameObject = Sphere;
            sphereController.SpherePool = GameManager.Instance.SpherePool;
            gManager.SphereControllers.Add(sphereController);
            gManager.scriptsBehaviourNoMono.Add(sphereController);
        }

        gManager.ballsInGame++;
        sphereController.InitialLaunch = false;
    }
}
