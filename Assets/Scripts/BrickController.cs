using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : ManagedUpdateBehaviour
{
    public Vector3 Size = Vector3.one;

    private void OnDrawGizmos()
    {
        if (gameObject.activeSelf)
            Gizmos.DrawWireCube(transform.position, Size);
    }

    public void CollideReaction() 
    {
        gameObject.SetActive(false);
    }
}
