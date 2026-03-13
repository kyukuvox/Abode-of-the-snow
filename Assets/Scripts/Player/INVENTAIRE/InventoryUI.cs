using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject slotPrefab;    // prefab : un Button avec une Image enfant
    public Transform slotsContainer; // panel horizontal en bas de l'écran

    void Awake() => Instance = this;

    void Start()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory introuvable !");
            return;
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        // On vide les anciens slots
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);

        // On crée un slot par objet dans l'inventaire
        foreach (InventoryItem item in Inventory.Instance.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotsContainer);

            // On affiche l'icône de l'objet
            slot.GetComponentInChildren<Image>().sprite = item.icon;

            // Au clic : on essaie de présenter l'objet au PNJ
            string capturedId = item.itemId;
            slot.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Clic sur : " + capturedId);
                NPCInteraction.Instance.TryPresentItem(capturedId);
            });
        }
    }
    //public void RefreshUI()
    //{
    //    foreach (Transform child in slotsContainer)
    //        Destroy(child.gameObject);

    //    Debug.Log("Nombre d'items dans l'inventaire : " + Inventory.Instance.items.Count);

    //    foreach (InventoryItem item in Inventory.Instance.items)
    //    {
    //        Debug.Log("Création du slot pour : " + item.itemId);
    //        GameObject slot = Instantiate(slotPrefab, slotsContainer);
    //        slot.GetComponentInChildren<Image>().sprite = item.icon;

    //        string capturedId = item.itemId;

    //        Button btn = slot.GetComponent<Button>();
    //        if (btn == null)
    //        {
    //            Debug.LogError("ERREUR : pas de Button sur la racine du slotPrefab !");
    //            return;
    //        }

    //        btn.onClick.AddListener(() =>
    //        {
    //            Debug.Log("Clic détecté sur : " + capturedId);
    //            NPCInteraction.Instance.TryPresentItem(capturedId);
    //        });
    //    }
    //}

}