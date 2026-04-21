using System.Collections;
using UnityEngine;

public class DraggableSprite : MonoBehaviour
{
    public GameObject hiddenItem;
    public GameObject hoverSprite;
    public int maxDrags = 3;
    public float triggerDistance = 2f;
    public float maxPullDistance = 1.5f; 
    public float resistanceStrength = 3f;

    private bool isDragging = false;
    private bool isActivated = false;
    private int currentDrags = 0;
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        if (hiddenItem != null)
            hiddenItem.SetActive(false);

        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isActivated) return;
        if (currentDrags >= maxDrags) return;

        if (hoverSprite != null)
            hoverSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (!isDragging)
            if (hoverSprite != null)
                hoverSprite.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isActivated) return;
        if (currentDrags >= maxDrags) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 direction = mousePosition - startPosition;
        float distance = direction.magnitude;

        if (distance > maxPullDistance)
        {
            float resistedDistance = maxPullDistance * (1f - 1f / (1f + distance / resistanceStrength));
            transform.position = startPosition + direction.normalized * resistedDistance;
        }
        else
        {
            transform.position = startPosition + direction;
        }

 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float maxAngle = 15f;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        float distance = Vector3.Distance(startPosition, transform.position);
        Debug.Log("Distance tirée : " + distance + " / " + triggerDistance);

        if (distance >= triggerDistance && !isActivated)
        {
            isActivated = true;
            if (hiddenItem != null)
                hiddenItem.SetActive(true);
            if (hoverSprite != null)
                hoverSprite.SetActive(false);

            StartCoroutine(ReturnToStart());
        }
        else
        {
            currentDrags++;
            Debug.Log("Tirages restants : " + (maxDrags - currentDrags));
            StartCoroutine(ReturnToStart());

            if (currentDrags >= maxDrags && !isActivated)
            {
                spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                if (hoverSprite != null)
                    hoverSprite.SetActive(false);
            }
        }
    }

    IEnumerator ReturnToStart()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(currentPos, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRot, Quaternion.identity, t);
            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxPullDistance);
    }
}