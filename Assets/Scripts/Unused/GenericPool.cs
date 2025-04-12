using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GenericPool<T> : MonoBehaviour
{
    public List<T> pool;


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

    //private GameObject CreatePooledItem()
    //{
    //    GameObject ball;

    //    //ball = Instantiate(, transform.position, Quaternion.identity);

    //    return ball;
    //}
}
