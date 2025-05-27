using UnityEngine;

public class TitleAnimation : ManagedUpdateBehaviourNoMono
{
    float RotationSpeed = -80;

    public override void UpdateMe()
    {
        gameObject.transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}
