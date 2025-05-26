using UnityEngine;

public class PowerUpController : ManagedUpdateBehaviourNoMono
{
    public float speedScale = 0;
    public Vector3 size = Vector3.one;
    public override void UpdateMe()
    {
        if (isPaused) return;

        Vector3 pos = gameObject.transform.position;

        pos.y += -1 * speedScale * Time.deltaTime;

        gameObject.transform.position = pos;
    }

    public virtual void PowerUpEffect() 
    {
        GameManager.Instance.IncreaseScore(100);

        GameManager.Instance.PlayAudio(GameManager.Instance.PowerUpClip);
    }
}
