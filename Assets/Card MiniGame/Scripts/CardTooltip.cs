using UnityEngine;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour
{
    public static CardTooltip Instance;

    public GameObject tooltipPanel;
    public Text tooltipText;

    void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false);

        Image img = tooltipPanel.GetComponent<Image>();
        if (img != null)
            img.raycastTarget = false;

        if (tooltipText != null)
            tooltipText.raycastTarget = false;
    }

    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;

            tooltipPanel.transform.position = new Vector2(
                mousePos.x + 80f,
                mousePos.y + 80f
            );
        }
    }

    public void Show(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}