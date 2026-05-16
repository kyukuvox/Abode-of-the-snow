using UnityEngine;
using System.Collections;

public class EndingTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        hasTriggered = true;
        StartCoroutine(TriggerEnding());
    }

    IEnumerator TriggerEnding()
    {
        bool fadeDone = false;
        FadeManager.Instance.FadeToBlackAndBack(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        });

        yield return null;
    }
}