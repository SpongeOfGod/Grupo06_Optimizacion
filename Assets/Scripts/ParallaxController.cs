using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParallaxController : ManagedUpdateBehaviourNoMono
{

    private Vector3 parallaxPosition;
    private float parallaxInitialPosition;

    private float parallaxLength;
    private float parallaxScale;
    private Vector3 playerPosition;
    


    public void InitializeParallax(float length, float scale, PlayerMovement player)
    {
        parallaxInitialPosition = 0;

        playerPosition = player.GameObject.transform.position;
        parallaxLength = length;
        parallaxScale = scale;
        
    }

    public override void UpdateMe()
    {
        Vector3 position = playerPosition;
        float temp = position.x * (1 -  parallaxScale);
        float distance = position.x * parallaxScale;

        Vector3 newPosition = new Vector3(parallaxInitialPosition + distance, parallaxPosition.y, parallaxPosition.z);

        parallaxPosition = newPosition;

        if (temp > parallaxInitialPosition + (parallaxScale / 2))
            parallaxInitialPosition += parallaxScale;

        if(temp <  parallaxInitialPosition - (parallaxScale / 2))
            parallaxInitialPosition -= parallaxScale;


        gameObject.transform.position = newPosition;
    }


}
