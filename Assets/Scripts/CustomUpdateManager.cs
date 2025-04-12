using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public List<ManagedUpdateBehaviourNoMono> scriptsBehaviourNoMono = new List<ManagedUpdateBehaviourNoMono>();

    public virtual void Update()
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
        }
    }
}
