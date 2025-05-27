using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : ManagedUpdateBehaviourNoMono
{
    public List<SplashText> splashTexts = new List<SplashText>();
    public float waitTime = 1f;
    public float elapsedTime = 0;
    private float fadeTime = 0.5f * 5f;
    public void Initialize()
    {
        foreach (var item in splashTexts)
        {
            item.Initialize();
        }
    }

    public override void UpdateMe()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= fadeTime + 1)
            SceneManager.LoadScene("MainMenu");

        if (elapsedTime >= fadeTime)
        {
            foreach (var item in splashTexts)
            {
                if (!item.fadeOut)
                {
                    item.FadeOut();
                }
            }
        }
        else
        {
            foreach (var item in splashTexts)
            {
                if (elapsedTime >= 0.5 && !item.fadeIn)
                {
                    item.FadeIn();
                }
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
