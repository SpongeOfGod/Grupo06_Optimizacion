using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class BrickPool : ManagedUpdateBehaviourNoMono
{
    [SerializeField] List<GameObject> brickControllers = new List<GameObject>();
    public ObjectPool<GameObject> Pool;
    public GameObject levelParent;

    public void InitializePool()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 50, 50);
    }
    private void OnDestroyPoolObject(GameObject Gobject)
    {
        //Destroy(Gobject);
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
        int index = Random.Range(0, brickControllers.Count);

        //GameObject brick = Instantiate(brickControllers[index], levelParent.transform);

        return null;
    }

}
