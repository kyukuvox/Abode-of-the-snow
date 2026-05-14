using System.Collections;
using UnityEngine;

public class ItemInteractableSprite : MonoBehaviour
{
    public Item requiredItem;
    public Sprite activatedSprite;

    public enum ActivationMode { SpriteChange, Animation }

    [Header("Mode activation")]
    public ActivationMode activationMode = ActivationMode.SpriteChange;

    [Header("Mode SpriteChange")]
    public GameObject animatedObject;
    public float targetYOffset = -3f;
    public float descendSpeed = 2f;

    [Header("Mode Animation")]
    public Animator targetAnimator;
    public string animationTrigger = "Activate";

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private HoverParticleManager hoverParticles;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isActivated) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    public void TryActivateWithItem(Item item)
    {
        Debug.Log("TryActivateWithItem appelé avec : " + (item != null ? item.itemName : "NULL"));
        Debug.Log("isActivated : " + isActivated);
        Debug.Log("requiredItem : " + (requiredItem != null ? requiredItem.itemName : "NULL"));

        if (isActivated) return;

        if (item == requiredItem)
        {
            Debug.Log("Item correct ! Mode : " + activationMode);
            isActivated = true;

            if (activatedSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = activatedSprite;

            if (hoverParticles != null)
                hoverParticles.Hide();

            if (Inventory.Instance.onItemChangedCallback != null)
                Inventory.Instance.onItemChangedCallback.Invoke();

            switch (activationMode)
            {
                case ActivationMode.SpriteChange:
                    Debug.Log("animatedObject : " + (animatedObject != null ? animatedObject.name : "NULL"));
                    if (animatedObject != null)
                        StartCoroutine(DescendObject());
                    break;

                case ActivationMode.Animation:
                    Debug.Log("Animator : " + (targetAnimator != null ? targetAnimator.name : "NULL"));
                    if (targetAnimator != null)
                        targetAnimator.SetTrigger(animationTrigger);
                    if (animatedObject != null)
                        StartCoroutine(DescendObject());
                    break;
            }
        }
        else
        {
            Debug.Log("Item incorrect ! Reçu : " + item.itemName + " | Requis : " + requiredItem.itemName);
            Inventory.Instance.AddItem(item);
        }
    }

    IEnumerator DescendObject()
    {
        Debug.Log("DescendObject lancé sur : " + animatedObject.name);

        Vector3 targetPosition = new Vector3(
            animatedObject.transform.position.x,
            animatedObject.transform.position.y + targetYOffset,
            animatedObject.transform.position.z
        );

        while (Vector3.Distance(animatedObject.transform.position, targetPosition) > 0.01f)
        {
            animatedObject.transform.position = Vector3.Lerp(
                animatedObject.transform.position,
                targetPosition,
                Time.deltaTime * descendSpeed
            );
            yield return null;
        }

        animatedObject.transform.position = targetPosition;
        Debug.Log("DescendObject terminé !");
    }

    void OnDrawGizmosSelected()
    {
        if (activationMode == ActivationMode.SpriteChange && animatedObject != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 target = new Vector3(
                animatedObject.transform.position.x,
                animatedObject.transform.position.y + targetYOffset,
                animatedObject.transform.position.z
            );
            Gizmos.DrawWireCube(target, Vector3.one * 0.5f);
            Gizmos.DrawLine(animatedObject.transform.position, target);
        }
    }
}