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

    public void Initialize()
    {
        initialColor = TMPro.color;
        Color endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }
    public override void UpdateMe()
    {
        if (fadeIn) 
        {
            TMPro.color = Color.Lerp(initialColor, endColor, Time.time / 0.5f);
        }
        else if (fadeOut) 
        {
            TMPro.color = Color.Lerp(endColor, initialColor, Time.time / 0.5f);
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
