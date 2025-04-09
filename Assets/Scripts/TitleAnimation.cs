using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : ManagedUpdateBehaviour
{
    [SerializeField] float RotationSpeed;

    public override void UpdateMe()
    {
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}
