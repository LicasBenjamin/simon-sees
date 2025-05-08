using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 0.75f;

    public void FadeToBlack()
    {
        StartCoroutine(FadeToBlackCoroutine());
    }

    IEnumerator FadeToBlackCoroutine()
    {
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0f, 0f, 0f, 1f);
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        fadeImage.color = targetColor;
    }
}
