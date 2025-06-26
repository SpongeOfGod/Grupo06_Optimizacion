using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : ManagedUpdateBehaviourNoMono
{
    private float parallaxLength;
    private float parallaxScale;

    private Transform playerTransform;
    private Vector3 startPosition;

    public void InitializeParallax(float length, float scale, PlayerMovement player)
    {
        parallaxLength = length;
        parallaxScale = scale;

        playerTransform = player.GameObject.transform;
        startPosition = gameObject.transform.position;
    }

    public override void UpdateMe()
    {
        if (GameManager.Instance == null || GameManager.Instance.Player == null)
            return;

        float deltaX = playerTransform.position.x * parallaxScale;
        Vector3 newPos = new Vector3(startPosition.x + deltaX, startPosition.y, startPosition.z);

        if (Mathf.Abs(playerTransform.position.x * (1 - parallaxScale)) > parallaxLength)
        {
            float offset = (playerTransform.position.x * (1 - parallaxScale)) % parallaxLength;
            newPos.x = startPosition.x + deltaX - offset;
        }

        gameObject.transform.position = newPos;
    }
}
