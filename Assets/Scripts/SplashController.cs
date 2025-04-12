using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : ManagedUpdateBehaviourNoMono
{
    public List<SplashText> splashTexts = new List<SplashText>();
    public float fadeTime;
    public float waitTime = 1f;
    public float elapsedTime = 0;
    public void Initialize()
    {

    }

    public override void UpdateMe()
    {
        elapsedTime += Time.deltaTime;

        foreach (var item in splashTexts)
        {
            if (elapsedTime >= 0.5 && !item.fadeIn)
            {
                elapsedTime = 0;
                item.FadeIn();
            }
        }
    }
    public void Splash() 
    {
        //yield return new WaitForSeconds(waitTime);

        //foreach (var item in splashTexts)
        //{
        //    StartCoroutine(item.FadeOut(fadeTime));
        //}
        //yield return new WaitForSeconds(fadeTime + waitTime);
        //StopAllCoroutines();
        //SceneManager.LoadScene("MainMenu");
    }
}
