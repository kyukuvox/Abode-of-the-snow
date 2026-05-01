using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Item myItem;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private GameObject ghostImage;
    private Vector2 ghostTargetPosition;
    private Coroutine ghostFollowCoroutine;

    public float followSpeed = 10f;
    public float maxTiltAngle = 20f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(Item item)
    {
        myItem = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (DialogueManager.Instance.IsActive()) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;


        EventSystem.current.SetSelectedGameObject(null);
        ItemSlotHover.IsDragging = true;

        Inventory.Instance.RemoveItemSilent(myItem);

        // Crée le fantôme
        ghostImage = Instantiate(gameObject, canvas.transform);
        ghostImage.name = "GhostItem";
        ghostImage.transform.SetAsLastSibling();

        Destroy(ghostImage.GetComponent<ItemSlotUI>());
        Destroy(ghostImage.GetComponent<ItemSlotHover>());

        CanvasGroup ghostCG = ghostImage.GetComponent<CanvasGroup>();
        if (ghostCG == null)
            ghostCG = ghostImage.AddComponent<CanvasGroup>();
        ghostCG.blocksRaycasts = false;
        ghostCG.alpha = 0.7f;

        RectTransform ghostRect = ghostImage.GetComponent<RectTransform>();
        ghostRect.pivot = new Vector2(0.5f, 0f);
        ghostRect.sizeDelta = rectTransform.sizeDelta;
        ghostImage.transform.localScale = Vector3.one * 0.8f;

        ghostTargetPosition = eventData.position;
        ghostImage.GetComponent<RectTransform>().position = eventData.position;

        ghostFollowCoroutine = StartCoroutine(GhostFollowMouse());

        // Cache le slot original
        LayoutElement le = GetComponent<LayoutElement>();
        if (le == null) le = gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = rectTransform.sizeDelta.x * 0.6f;
        le.preferredHeight = rectTransform.sizeDelta.y * 0.6f;
        le.minWidth = rectTransform.sizeDelta.x * 0.6f;
        le.minHeight = rectTransform.sizeDelta.y * 0.6f;
        transform.localScale = Vector3.one * 0.6f;
        canvasGroup.alpha = 0.3f;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            transform.parent.GetComponent<RectTransform>()
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        ghostTargetPosition = eventData.position;
    }

    IEnumerator GhostFollowMouse()
    {
        RectTransform ghostRect = ghostImage.GetComponent<RectTransform>();
        Vector2 previousPos = ghostRect.position;

        while (ghostImage != null)
        {
            Vector2 currentPos = ghostRect.position;

            ghostRect.position = Vector2.Lerp(
                currentPos,
                ghostTargetPosition,
                Time.deltaTime * followSpeed
            );

            Vector2 moveDir = (Vector2)ghostRect.position - previousPos;

            if (moveDir.magnitude > 0.1f)
            {
                float tiltAngle = Mathf.Clamp(-moveDir.x * 2f, -maxTiltAngle, maxTiltAngle);
                ghostImage.transform.rotation = Quaternion.Lerp(
                    ghostImage.transform.rotation,
                    Quaternion.Euler(0, 0, tiltAngle),
                    Time.deltaTime * followSpeed * 0.5f
                );
            }
            else
            {
                ghostImage.transform.rotation = Quaternion.Lerp(
                    ghostImage.transform.rotation,
                    Quaternion.identity,
                    Time.deltaTime * followSpeed
                );
            }

            previousPos = ghostRect.position;
            yield return null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ItemSlotHover.IsDragging = false;

        if (ghostFollowCoroutine != null)
            StopCoroutine(ghostFollowCoroutine);

        if (ghostImage != null)
            Destroy(ghostImage);

        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null)
            Destroy(le);

        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            transform.parent.GetComponent<RectTransform>()
        );

        EventSystem.current.SetSelectedGameObject(null);
        NPCDropTarget npc = GetNPCUnderCursor();
        PortalDropTarget portal = GetPortalUnderCursor();
        ItemInteractableDropTarget interactable = GetInteractableUnderCursor();
        ItemSpriteInteractionDropTarget spriteInteraction = GetSpriteInteractionUnderCursor();

        if (npc != null)
            npc.ReceiveDroppedItem(myItem);
        else if (portal != null)
            portal.ReceiveDroppedItem(myItem);
        else if (interactable != null)
            interactable.ReceiveDroppedItem(myItem);
        else if (spriteInteraction != null)
            spriteInteraction.ReceiveDroppedItem(myItem);
        else
            Inventory.Instance.AddItem(myItem);
    }
    ItemSpriteInteractionDropTarget GetSpriteInteractionUnderCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null)
            return hit.GetComponent<ItemSpriteInteractionDropTarget>();

        return null;
    }
    ItemInteractableDropTarget GetInteractableUnderCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null)
            return hit.GetComponent<ItemInteractableDropTarget>();

        return null;
    }


    NPCDropTarget GetNPCUnderCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null)
            return hit.GetComponent<NPCDropTarget>();

        return null;
    }

    PortalDropTarget GetPortalUnderCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null)
            return hit.GetComponent<PortalDropTarget>();

        return null;
    }
}