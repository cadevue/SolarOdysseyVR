using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    //public CanvasGroup canvasGroup;
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(255, 255, 255, t / fadeDuration);
            //canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
        //canvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeImage.color = new Color(255, 255, 255, t / fadeDuration);
            //canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        //canvasGroup.alpha = 0;
        fadeImage.gameObject.SetActive(false);
    }
}
