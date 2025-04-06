using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : ManagedUpdateBehaviour
{
    public List<SplashText> splashTexts = new List<SplashText>();
    public float fadeTime;
    public float waitTime = 1f;
    event Action splashEnd;
    private void Awake()
    {
        StartCoroutine(Splash());
    }

    IEnumerator Splash() 
    {
        foreach (var item in splashTexts)
        {
            StartCoroutine(item.FadeIn(fadeTime));
            yield return new WaitForSeconds(waitTime);
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var item in splashTexts)
        {
            StartCoroutine(item.FadeOut(fadeTime));
        }
        yield return new WaitForSeconds(fadeTime + waitTime);
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
    }
}
