using System.Collections;
using UnityEngine;

public class ClickCounter : MonoBehaviour
{
    public GameObject hiddenItem;
    public GameObject interactionSprite; 
    public int clicksRequired = 3;

    private int currentClicks = 0;
    private bool isActivated = false;
    private bool isOnCooldown = false;

    void Start()
    {
        if (hiddenItem != null)
            hiddenItem.SetActive(false);

        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isActivated) return;

        if (interactionSprite != null)
            interactionSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isActivated) return;
        if (isOnCooldown) return; 
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        currentClicks++;
        Debug.Log("Clics : " + currentClicks + "/" + clicksRequired);

        StartCoroutine(ClickCooldown());

        if (currentClicks >= clicksRequired)
        {
            isActivated = true;
            if (hiddenItem != null)
                hiddenItem.SetActive(true);
            if (interactionSprite != null)
                interactionSprite.SetActive(false);
        }
    }

    IEnumerator ClickCooldown()
    {
        isOnCooldown = true;

        if (interactionSprite != null)
            interactionSprite.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        isOnCooldown = false;

        if (interactionSprite != null)
            interactionSprite.SetActive(true);
    }
}