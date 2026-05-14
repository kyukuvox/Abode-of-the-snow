using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadePanel;
    public float fadeDuration = 0.5f;
    public float sceneStartFadeDuration = 1f; 

    void Awake()
    {
        Instance = this;
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
    }

    void Start()
    {
        StartCoroutine(FadeInOnStart());
    }

    IEnumerator FadeInOnStart()
    {
        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;
        while (elapsed < sceneStartFadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsed / sceneStartFadeDuration));
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
}