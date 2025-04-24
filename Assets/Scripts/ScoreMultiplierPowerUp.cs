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
        GameManager.Instance.ApplyScoreMultiplier(newMultiplier, duration);
    }


    /* Variables nuevas 
    
    public float scoreMultiplier = 1f;





    // Cambio en el método de sumar score


     public void IncreaseScore(int amount)
    {
        int finalScore = Mathf.RoundToInt(amount * scoreMultiplier);
        score += finalScore;
        ScoreCount.text = score.ToString();
    }



    // Switch de Power up

    case 6:
                powerUpController = new ScoreMultiplierPowerUp();
                powerUpController.GameObject = powerUp;
                BallMaterialBlock.SetColor("_Color", Color.white);
                break;

     



    // Método para iniciar el power up

     public void ApplyScoreMultiplier(float amount, float duration)
    {
        StartCoroutine(ScoreMultiplierBuff(amount, duration));
    }





    // IEnumerator

     IEnumerator ScoreMultiplierBuff(float multiplier, float duration)
    {
        scoreMultiplier = multiplier;

        Color colorA = Color.red;
        Color colorB = Color.yellow;
        float flickerSpeed = 5f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.PingPong(Time.time * flickerSpeed, 1f);
            ScoreCount.color = Color.Lerp(colorA, colorB, t);

            yield return null;
        }

        scoreMultiplier = 1f;
        ScoreCount.color = Color.white;
    }

    */
}
