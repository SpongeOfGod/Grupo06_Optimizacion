using UnityEngine;
using UnityEngine.Pool;

public class SphereController : ManagedUpdateBehaviourNoMono
{
    public Vector2 MoveDirection;
    public Vector2 MoveSpeed = new Vector2(12, 12);
    public bool InitialLaunch;
    public float Radius = 0.25f;
    public float Mass = 20;
    public float InitialYOffset = -3.93f;
    public PlayerMovement player;
    private bool bounceOnce;
    public Vector2 InitialSpeed;
    public ObjectPool<GameObject> SpherePool;
    public TrailRenderer trailRenderer;
    public bool fireBallMode;

    public void LaunchDirection(Vector3 Movedirection)
    {
        InitialSpeed = MoveSpeed;
        InitialLaunch = true;
        MoveDirection = Movedirection;
    }

    public override void UpdateMe()
    {
        if (!InitialLaunch || !gameObject.activeSelf) return;

        BallMovement();

        if (!fireBallMode)
            trailRenderer.Clear();
    }

    private void BallMovement()
    {
        gameObject.transform.position += new Vector3(MoveDirection.x * MoveSpeed.x * Time.deltaTime, MoveDirection.y * MoveSpeed.y * Time.deltaTime);
    }
}
