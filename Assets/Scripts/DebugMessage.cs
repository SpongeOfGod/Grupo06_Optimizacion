using UnityEngine;

public class DebugMessage : MonoBehaviour, IManualUpdate
{
    public void ManualUpdate()
    {
        Debug.Log("hi");
    }
}
