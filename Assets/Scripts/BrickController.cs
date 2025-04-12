using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : ManagedUpdateBehaviourNoMono
{
    public Vector3 Size = Vector3.one;

    public void CollideReaction() 
    {
        gameObject.SetActive(false);
    }
}
