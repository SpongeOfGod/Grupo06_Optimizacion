using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    List<ManagedUpdateBehaviour> scriptsBehaviour = new List<ManagedUpdateBehaviour>();

    private void Awake()
    {
        scriptsBehaviour = GetComponents<ManagedUpdateBehaviour>().ToList();
    }

    private void Update()
    {
        int count = scriptsBehaviour.Count;
        for (int i = 0; i < count; i++)
        {
            scriptsBehaviour[i].UpdateMe();
        }
    }
}
