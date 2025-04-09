using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public List<ManagedUpdateBehaviour> scriptsBehaviour = new List<ManagedUpdateBehaviour>();
    public static CustomUpdateManager Instance = null; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        for (int i = 0; i < scriptsBehaviour.Count; i++)
        {
            if (scriptsBehaviour[i] == null) 
            {
                scriptsBehaviour.RemoveAt(i);
                break;
            }

            if (scriptsBehaviour[i].gameObject.activeSelf)
                scriptsBehaviour[i].UpdateMe();
        }
    }
}
