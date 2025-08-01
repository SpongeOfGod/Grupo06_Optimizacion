using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BrickController : ManagedUpdateBehaviourNoMono
{
    public ObjectPool<GameObject> MyPool;
    public MeshFilter meshFilter;
    public Vector3 Size = Vector3.one;
    public PowerUpController powerUp;
    public Color brickColor = Color.red;

    public Color PowerUpColor;

    public int Durability = 1;
    public MaterialPropertyBlock MaterialPropertyBlock;
    public GameObject powerUpObject;

    public void CollideReaction()
    {
        Durability--;

        if (Durability >= 1) 
        {
            VisualEffects(25);
            return;
        }

        if (powerUp != null)
        {
            if (powerUp.speedScale == 0)
                powerUp.ChangeVisual();
            ////Debug.Log($"Release Power up {Random.Range(0, 10)}");
            //GameManager.Instance.PlayAudio(GameManager.Instance.PowerUpSpawnClip);
            //powerUp.GameObject.SetActive(true);
            //if ((new Vector3(3, 3, 3)).magnitude < (powerUp.GameObject.transform.localScale).magnitude)
            //    powerUp.GameObject.transform.localScale = powerUp.GameObject.transform.localScale + new Vector3(3, 3, 3);
            //else
            //    powerUp.GameObject.transform.localScale = powerUp.GameObject.transform.localScale + new Vector3(1, 1, 1);
            //powerUp.GameObject.transform.SetParent(GameManager.Instance.powerUpParent.transform);
            //powerUp.speedScale = 5f;
            //Renderer renderer = powerUp.GameObject.GetComponent<Renderer>();
            //MaterialPropertyBlock MaterialPower = new MaterialPropertyBlock();
            //renderer.GetPropertyBlock(MaterialPower);
            //MaterialPower.SetColor("_Color", PowerUpColor);
            //renderer.SetPropertyBlock(MaterialPower);
        }

        VisualEffects(50);

        powerUp = null;

        if (gameObject != null)
            MyPool.Release(gameObject);
    }

    public void RefreshBrick() 
    {
        if (meshFilter == null && gameObject != null) 
        {
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = GameManager.Instance.GetBrickVariation(this, Durability);
        }
    }

    private void VisualEffects(int value)
    {
        if (gameObject != null) 
        {
            GameManager.Instance.LevelManager.BricksMaterial.TryGetValue(gameObject, out Renderer renderer);

            if (renderer != null)
                renderer.GetPropertyBlock(MaterialPropertyBlock);

            //meshFilter.mesh = GameManager.Instance.GetBrickVariation(this, Mathf.Clamp(Durability - 1, 0, 10));
            renderer = gameObject.GetComponent<Renderer>();
            MaterialPropertyBlock block = new();
            renderer.GetPropertyBlock(block);
            block.SetColor("_Color", MaterialPropertyBlock.GetColor("_Color"));
            renderer.SetPropertyBlock(block);
            brickColor = block.GetColor("_Color");
            GameObject Particles = GameManager.Instance.particlePool.GetParticles();
            Particles.gameObject.transform.position = gameObject.transform.position;
            GameManager.Instance.GiveColorParticle(Particles, brickColor);
            Particles.gameObject.SetActive(true);
            GameManager.Instance.IncreaseScore(value);
        }
    }
}
   
