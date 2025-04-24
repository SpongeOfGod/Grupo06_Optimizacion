using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : ManagedUpdateBehaviourNoMono
{
    public int poolSize = 10;

    private List<GameObject> pool;
    public void InitializePool()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject Particles = GameManager.Instance.SpawnDestroyParticles();
            Particles.SetActive(false);
            pool.Add(Particles);
        }
    }

    public GameObject GetParticles() 
    {
        GameObject particle = null;

        if (pool.Count > 0) 
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].activeSelf == false) 
                {
                    particle = pool[i];
                }
            }

            if (particle != null)
                return particle;
        }
        
        if (pool.Count == 0 || particle == null)
        {
            particle = GameManager.Instance.SpawnDestroyParticles();
            pool.Add(particle);
            return particle;
        }

        return null;
    }
}
