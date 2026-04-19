using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadePanel;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        Instance = this;
        fadePanel.gameObject.SetActive(false);
    }

    public void FadeToBlackAndBack(System.Action onBlack)
    {
        StartCoroutine(FadeRoutine(onBlack));
    }

    IEnumerator FadeRoutine(System.Action onBlack)
    {
        fadePanel.gameObject.SetActive(true);

        // Fondu vers le noir
        float elapsed = 0f;
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 1f);

        onBlack?.Invoke();

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, 1f - elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.gameObject.SetActive(false);
    }
}