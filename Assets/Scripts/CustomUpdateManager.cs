using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public List<ManagedUpdateBehaviourNoMono> scriptsBehaviourNoMono = new List<ManagedUpdateBehaviourNoMono>();

    public virtual void Update()
    {
        for (int i = scriptsBehaviourNoMono.Count - 1; i >= 0; i--)
        {
            var behaviour = scriptsBehaviourNoMono[i];

            if (behaviour == null || behaviour.GameObject == null)
            {
                scriptsBehaviourNoMono.RemoveAt(i);
            }
            else
            {
                behaviour.UpdateMe();
            }
        }
    }
}
 