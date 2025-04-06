using System.Collections;
using TMPro;
using UnityEngine;

public class SplashText : ManagedUpdateBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public IEnumerator FadeIn(float secondsToEnd) 
    {
        float elapsedTime = 0;
        Color initialColor = text.color;
        Color endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        while (elapsedTime < secondsToEnd) 
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(initialColor, endColor, elapsedTime / secondsToEnd);
            yield return null;
        }
        text.color = endColor;
    }

    public IEnumerator FadeOut(float secondsToEnd)
    {
        float elapsedTime = 0;
        Color initialColor = text.color;
        Color endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        while (elapsedTime < secondsToEnd)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(initialColor, endColor, elapsedTime / secondsToEnd);
            yield return null;
        }
        text.color = endColor;
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
