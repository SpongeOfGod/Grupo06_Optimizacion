using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public List<ManagedUpdateBehaviour> scriptsBehaviour = new List<ManagedUpdateBehaviour>();
    public List<ManagedUpdateBehaviourNoMono> scriptsBehaviourNoMono = new List<ManagedUpdateBehaviourNoMono>();
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
        for (int i = 0; i < scriptsBehaviourNoMono.Count; i++)
        {
            if (scriptsBehaviourNoMono[i] == null)
            {
                scriptsBehaviourNoMono.RemoveAt(i);
                break;
            }
            else 
            {
                scriptsBehaviourNoMono[i].UpdateMe();
            }

            //if (scriptsBehaviourNoMono[i].gameObject.activeSelf)
        }

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
