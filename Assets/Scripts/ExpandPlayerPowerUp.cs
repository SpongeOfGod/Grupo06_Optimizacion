using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandPlayerPowerUp : PowerUpController
{
   public float sizeMultiplier = 3.5f;

    public override void PowerUpEffect()
    {
        PlayerMovement player = GameManager.Instance.Player;

        player.Size = new Vector3(player.Size.x * sizeMultiplier, player.Size.y, player.Size.z); // Aumenta el tamaño del jugador
        
        player.GameObject.transform.localScale = new Vector3
            (
                player.GameObject.transform.localScale.x * sizeMultiplier,
                player.GameObject.transform.localScale.y, 
                player.GameObject.transform.localScale.z
            ); // Ajusta la escala visual
    }

}
