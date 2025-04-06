using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CustomUpdateManager))]
public abstract class ManagedUpdateBehaviour : MonoBehaviour
{
    public virtual void UpdateMe() 
    {
    
    }
}
