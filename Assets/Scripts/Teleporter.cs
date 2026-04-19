using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform destination;
    public GameObject hoverSprite;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

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

        if (destination != null)
        {
            FadeManager.Instance.FadeToBlackAndBack(() =>
            {
                player.position = destination.position;
            });
        }
    }

    void OnDrawGizmosSelected()
    {
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(destination.position, 0.3f);
            Gizmos.DrawLine(transform.position, destination.position);
        }
    }
}