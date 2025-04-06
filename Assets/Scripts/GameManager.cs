using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 XScreenThresshold;
    public Vector2 YScreenThresshold;
    public PlayerMovement Player;
    public SphereController Sphere;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        } else if (Instance != this) 
        {
            Destroy(gameObject);
        }

    }
}
