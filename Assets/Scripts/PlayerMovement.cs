using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : ManagedUpdateBehaviour
{
    [SerializeField] float speedScale;

    public Vector3 direction;

    public Vector3 Size;

    public Vector3 offset1 = Vector3.zero;
    public Vector3 offset2 = Vector3.zero;
    public Vector3 offset3 = Vector3.zero;

    public override void UpdateMe()
    {
        float xInput = Input.GetAxis("Horizontal");
        if (xInput != 0) 
        {
            Vector3 pos = transform.position;
            pos.x += xInput * Time.deltaTime * speedScale;

            if (pos.x < GameManager.Instance.XScreenThresshold.x + Size.x / 2)
                pos.x = GameManager.Instance.XScreenThresshold.x + Size.x / 2;
            else if (pos.x > GameManager.Instance.XScreenThresshold.y - Size.x / 2)
                pos.x = GameManager.Instance.XScreenThresshold.y - Size.x / 2;

            transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, direction);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + offset1, transform.position + offset1 + Vector3.up);
        Gizmos.DrawLine(transform.position + offset2, transform.position + offset2 + Vector3.up);
        Gizmos.DrawLine(transform.position + offset3, transform.position + offset3 + Vector3.up);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, Size);
    }
}
