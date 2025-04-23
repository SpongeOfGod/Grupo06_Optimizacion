using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagedUpdateBehaviourNoMono
{
    private int numberOfBricks;
    private Dictionary<Vector2, GameObject> grid = new Dictionary<Vector2, GameObject>();
    private Dictionary<GameObject, Renderer> bricksMaterial = new Dictionary<GameObject, Renderer>();
    private Dictionary<GameObject, BrickController> brickToController = new Dictionary<GameObject, BrickController>();

    public List<BrickController> Bricks = new List<BrickController>();

    GameManager gManager;
    int powerUpCount = 0;

    public void InitializeLevel()
    {
        if (!gManager)
            gManager = GameManager.Instance;

        GameManager.Instance.levelParent.gameObject.SetActive(false);

        Color[] selectedGradient = gManager.GetRandomGradient();

        for (int i = 0; i < gManager.BrickPositions.Count; i++)
        {
            gManager.InitializePool();
            GameObject brick = gManager.BrickPool.Get();
            brick.transform.position = gManager.BrickPositions[i];

            BrickController brickController = new BrickController();
            brickController.GameObject = brick;

            PowerUpController powerUpController = null;
            if (powerUpCount < 3 && Random.value > 0.70f)
            {
                powerUpController = gManager.CreatePowerUp(brickController.GameObject.transform.position);
                powerUpCount++;
            }

            if (powerUpController != null)
                brickController.powerUp = powerUpController;

            Renderer renderer = brick.GetComponent<Renderer>();
            gManager.Bricks.Add(brickController);
            Bricks.Add(brickController);

            bricksMaterial.TryAdd(brick, renderer);
            brickToController.TryAdd(brick, brickController);

            numberOfBricks++;

            SetPositionAndColor(brick, gManager.BrickPositions[i], selectedGradient);
        }

        GameManager.Instance.levelParent.transform.position = new Vector3(0, 4, 0);
        GameManager.Instance.levelParent.gameObject.SetActive(true);
        GameManager.Instance.LevelAppear();
    }

    public override void UpdateMe()
    {
        if (gManager.ballsInGame == 0)
            CreateSphere();

        bool anyBrickActive = false;

        foreach (var item in gManager.Bricks)
        {
            if (item.GameObject.activeSelf)
                anyBrickActive = true;
        }

        if (!anyBrickActive)
        {
            Color[] selectedGradient = gManager.GetRandomGradient();
            powerUpCount = 0;
            GameManager.Instance.IncreaseLevel();
            GameManager.Instance.levelParent.transform.position = new Vector3(0, 4, 0);

            foreach (var item in gManager.Bricks)
            {
                numberOfBricks = 0;
                item.GameObject.SetActive(true);

                PowerUpController powerUpController = null;
                if (powerUpCount < 3 && Random.value > 0.70f)
                {
                    powerUpCount++;
                    powerUpController = gManager.CreatePowerUp(item.GameObject.transform.position);
                }

                if (powerUpController != null)
                {
                    item.powerUp = powerUpController;
                }

                SetPositionAndColor(item.GameObject, item.GameObject.transform.position, selectedGradient);
            }

            GameManager.Instance.LevelAppear();
        }
    }

    private void SetPositionAndColor(GameObject brick, Vector3 position, Color[] gradient)
    {
        if (!grid.ContainsKey(position))
        {
            brick.transform.position = position;
            grid.Add(position, brick);
        }

        bricksMaterial.TryGetValue(brick, out Renderer renderer);

        float minY = 1.9f;
        float maxY = 3.75f;
        float t = Mathf.InverseLerp(minY, maxY, position.y);

        Color interpolatedColor = Color.Lerp(gradient[0], gradient[1], t);

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", interpolatedColor);
        renderer.SetPropertyBlock(propertyBlock);

        if (brickToController.TryGetValue(brick, out var controller))
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
