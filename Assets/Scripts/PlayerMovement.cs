using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : ManagedUpdateBehaviourNoMono
{
    [SerializeField] float speedScale = 10.11f;

    public Vector3 Size = new Vector3(2.19f, 0.2f, 2f);
    public override void UpdateMe()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0) 
        {
            Vector3 pos = gameObject.transform.position;
            pos.x += xInput * Time.deltaTime * speedScale;


            if (pos.x < GameManager.Instance.XScreenThresshold.x + Size.x / 2)
                pos.x = GameManager.Instance.XScreenThresshold.x + Size.x / 2;
            else if (pos.x > GameManager.Instance.XScreenThresshold.y - Size.x / 2)
                pos.x = GameManager.Instance.XScreenThresshold.y - Size.x / 2;

            gameObject.transform.position = pos;
        }
    }
}
