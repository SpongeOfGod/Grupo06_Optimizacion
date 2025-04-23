using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : ManagedUpdateBehaviourNoMono
{
    [SerializeField] float speedScale = 10.11f;
    public Vector3 Size = new Vector3(2.19f, 0.2f, 2f);
    public bool fireBallPad;
    public bool canXplode = true;
    private int lastDirection = 0;

    public override void UpdateMe()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            lastDirection = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            lastDirection = 1;
        }

        if ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)) && lastDirection == 1)
        {
            if (IsKeyHeld(KeyCode.LeftArrow, KeyCode.A))
                lastDirection = -1;
            else
                lastDirection = 0;
        }
        else if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) && lastDirection == -1)
        {
            if (IsKeyHeld(KeyCode.RightArrow, KeyCode.D))
                lastDirection = 1;
            else
                lastDirection = 0;
        }

        if (lastDirection != 0)
        {
            Vector3 pos = gameObject.transform.position;
            pos.x += lastDirection * Time.deltaTime * speedScale;

            float minX = GameManager.Instance.XScreenThresshold.x + Size.x / 2;
            float maxX = GameManager.Instance.XScreenThresshold.y - Size.x / 2;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            gameObject.transform.position = pos;
        }
    }

    private bool IsKeyHeld(KeyCode key1, KeyCode key2)
    {
        return Input.GetKey(key1) || Input.GetKey(key2);
    }
}
