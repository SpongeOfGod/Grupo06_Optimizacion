using UnityEngine;

public abstract class ManagedUpdateBehaviourNoMono
{
    protected GameObject gameObject;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public bool isPaused;
    public virtual void UpdateMe()
    {

    }
}
