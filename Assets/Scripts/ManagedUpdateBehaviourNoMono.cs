using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagedUpdateBehaviourNoMono
{
    protected GameObject gameObject;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public virtual void UpdateMe()
    {

    }
}
