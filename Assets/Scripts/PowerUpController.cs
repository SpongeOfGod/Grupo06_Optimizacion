using UnityEngine;

public class PowerUpController : ManagedUpdateBehaviourNoMono
{
    public float speedScale = 0;
    public Vector3 size = Vector3.one;
    public GameObject brick;
    public Color PowerUpColor;
    public bool VisualChange;
    public override void UpdateMe()
    {
        if ((brick == null || !brick.activeSelf) && speedScale == 0 && !VisualChange) 
        {
            ChangeVisual();
        }

        if (isPaused) return;

        Vector3 pos = gameObject.transform.position;

        pos.y += -1 * speedScale * Time.deltaTime;

        gameObject.transform.position = pos;
    }

    public void ChangeVisual() 
    {
        speedScale = 5f;
        VisualChange = true;
        GameManager.Instance.PlayAudio(GameManager.Instance.PowerUpSpawnClip);

        if (GameObject != null) 
        {
            if ((new Vector3(3, 3, 3)).magnitude < (GameObject.transform.localScale).magnitude)
                GameObject.transform.localScale = GameObject.transform.localScale + new Vector3(3, 3, 3);
            else
                GameObject.transform.localScale = GameObject.transform.localScale + new Vector3(1, 1, 1);

            GameObject.transform.SetParent(GameManager.Instance.powerUpParent.transform);
            Renderer renderer = GameObject.GetComponent<Renderer>();
            MaterialPropertyBlock MaterialPower = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(MaterialPower);
            MaterialPower.SetColor("_Color", PowerUpColor);
            renderer.SetPropertyBlock(MaterialPower);
        }
    }

    public virtual void PowerUpEffect() 
    {
        GameManager.Instance.IncreaseScore(100);

        GameManager.Instance.PlayAudio(GameManager.Instance.PowerUpClip);
    }
}
