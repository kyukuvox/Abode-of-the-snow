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

    void Start()
    {
        StartCoroutine(FadeInOnStart());
    }

    IEnumerator FadeInOnStart()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsed / fadeDuration));
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.gameObject.SetActive(false);
    }

    public void FadeToBlackAndBack(System.Action onBlack)
    {
        StartCoroutine(FadeRoutine(onBlack));
    }

    IEnumerator FadeRoutine(System.Action onBlack)
    {
        fadePanel.gameObject.SetActive(true);

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

    public IEnumerator FadeToBlack(float duration)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, elapsed / duration));
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsed / duration));
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.gameObject.SetActive(false);
    }
}