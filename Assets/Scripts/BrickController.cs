using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BrickController : ManagedUpdateBehaviourNoMono
{
    public ObjectPool<GameObject> MyPool;
    public Vector3 Size = Vector3.one;
    public PowerUpController powerUp;
    public Color brickColor = Color.red;

    public Color PowerUpColor;

    public int Durability = 1;

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
            powerUp.GameObject.SetActive(true);
            if ((new Vector3(3, 3, 3)).magnitude < (powerUp.GameObject.transform.localScale).magnitude)
                powerUp.GameObject.transform.localScale = powerUp.GameObject.transform.localScale + new Vector3(3, 3, 3);
            else
                powerUp.GameObject.transform.localScale = powerUp.GameObject.transform.localScale + new Vector3(1, 1, 1);
            powerUp.GameObject.transform.SetParent(GameManager.Instance.powerUpParent.transform);
            powerUp.speedScale = 5f;
            Renderer renderer = powerUp.GameObject.GetComponent<Renderer>();
            MaterialPropertyBlock MaterialPower = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(MaterialPower);
            MaterialPower.SetColor("_Color", PowerUpColor);
            renderer.SetPropertyBlock(MaterialPower);
        }

        VisualEffects(50);

        powerUp = null;
        MyPool.Release(gameObject);
    }

    private void VisualEffects(int value)
    {
        GameObject Particles = GameManager.Instance.particlePool.GetParticles();
        Particles.gameObject.transform.position = gameObject.transform.position;
        GameManager.Instance.GiveColorParticle(Particles, brickColor);
        Particles.gameObject.SetActive(true);
        GameManager.Instance.IncreaseScore(value);
    }
}
   
