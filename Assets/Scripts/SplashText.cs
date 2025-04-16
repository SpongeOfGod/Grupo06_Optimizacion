using System.Collections;
using TMPro;
using UnityEngine;

public class SplashText : ManagedUpdateBehaviourNoMono
{
    public TextMeshProUGUI TMPro;
    Color initialColor;
    Color endColor;
    public bool fadeIn;
    public bool fadeOut;
    private float elapsedFadeInTime = 0;
    private float elapsedFadeOutTime = 0;
    public void Initialize()
    {
        initialColor = TMPro.color;
        endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }
    public override void UpdateMe()
    {
        if (fadeIn && elapsedFadeInTime < 0.5f) 
        {
            elapsedFadeInTime += Time.deltaTime;
            TMPro.color = Color.Lerp(initialColor, endColor, elapsedFadeInTime / 0.5f);
        }
        else if (fadeOut && elapsedFadeOutTime < 0.5f) 
        {
            elapsedFadeOutTime += Time.deltaTime;
            TMPro.color = Color.Lerp(endColor, initialColor, elapsedFadeOutTime / 0.5f);
        }
    }
    public void FadeIn() 
    {
        fadeIn = true;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }
}
