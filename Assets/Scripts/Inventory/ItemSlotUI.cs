using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Item myItem;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private GameObject ghostImage;

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

        EventSystem.current.SetSelectedGameObject(null);

        Inventory.Instance.RemoveItemSilent(myItem);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

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
        ghostRect.pivot = new Vector2(0.5f, 0.5f);
        ghostRect.sizeDelta = rectTransform.sizeDelta;
        ghostImage.transform.localScale = Vector3.one * 0.8f;

        MoveGhostToMouse(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostImage != null)
            MoveGhostToMouse(eventData);
    }

    void MoveGhostToMouse(PointerEventData eventData)
    {
        RectTransform ghostRect = ghostImage.GetComponent<RectTransform>();
        ghostRect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostImage != null)
            Destroy(ghostImage);

        EventSystem.current.SetSelectedGameObject(null);

        NPCDropTarget npc = GetNPCUnderCursor();

        if (npc != null)
        {
            npc.ReceiveDroppedItem(myItem);
        }
        else
        {
            Inventory.Instance.AddItem(myItem);
        }
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
}