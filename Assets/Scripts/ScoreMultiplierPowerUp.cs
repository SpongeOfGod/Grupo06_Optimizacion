using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMultiplierPowerUp : PowerUpController
{
    float newMultiplier = 2f;
    float duration = 10f;

    public override void PowerUpEffect()
    {
        base.PowerUpEffect();
        GameManager.Instance.AddOrRefreshPowerUp(this, 3f);

        GameManager.Instance.ApplyScoreMultiplier(newMultiplier, duration, this);
    }


    /* Variables nuevas 
    






    // Cambio en el método de sumar score






    // Switch de Power up

    
    // Método para iniciar el power up

     





    // IEnumerator

    
    */
}
