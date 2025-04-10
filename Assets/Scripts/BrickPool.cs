using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class BrickPool : MonoBehaviour
{
    [SerializeField] List<BrickController> brickControllers = new List<BrickController>();
    public ObjectPool<GameObject> Pool;

    private void Awake()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);
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
        int index = Random.Range(0, brickControllers.Count);

        GameObject brick = Instantiate(brickControllers[index].gameObject, transform.position, Quaternion.identity);

        return brick;
    }

}
