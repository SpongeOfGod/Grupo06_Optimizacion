using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagedUpdateBehaviourNoMono
{
    private int numberOfBricks;
    private Dictionary<Vector2, GameObject> grid = new Dictionary<Vector2, GameObject>();
    private Dictionary<GameObject, Renderer> bricksMaterial = new Dictionary<GameObject, Renderer>();
    public List<BrickController> Bricks = new List<BrickController>();

    GameManager gManager;

    public void InitializeLevel()
    {
        if (!gManager)
            gManager = GameManager.Instance;

        for (int i = 0; i < gManager.BrickPositions.Count; i++)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            gManager.InitializePool();
            GameObject brick = gManager.BrickPool.Get();
            brick.transform.position = gManager.BrickPositions[i];
            BrickController brickController = new BrickController();
            brickController.GameObject = brick;
            Renderer renderer = brick.GetComponent<Renderer>();
            gManager.Bricks.Add(brickController);

            bricksMaterial.TryAdd(brick, renderer);
            numberOfBricks++;

            SetPositionAndColor(brick, gManager.BrickPositions[i]);
        }
    }

    public override void UpdateMe()
    {
        if (gManager.ballsInGame == 0)
            CreateSphere();

        bool allGameObjectsOff = false;

        foreach (var item in gManager.Bricks)
        {
            if (item.GameObject.activeSelf)
                allGameObjectsOff = true;
        }

        if (!allGameObjectsOff)
        {
            foreach (var item in gManager.Bricks)
            {
                numberOfBricks = 0;
                item.GameObject.SetActive(true);
            }
        }
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

    public void CreateSphere()
    {
        GameObject Sphere = gManager.SpherePool.Get();
        bool SphereExisted = false;
        SphereController sphereController = new SphereController();

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

//playersquare