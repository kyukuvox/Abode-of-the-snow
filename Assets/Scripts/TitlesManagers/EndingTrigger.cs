using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public GameObject hoverSprite;

    void Start()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (hoverSprite != null)
            hoverSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseDown()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        FadeManager.Instance.FadeToBlackAndBack(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        });
    }
}