using System.Collections;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private const string SAVE_KEY = "SaveData";
    private const string LOAD_FLAG = "ShouldLoad";

    void Start()
    {
        Debug.Log("=== GAME LOADER ===");
        Debug.Log("Load flag : " + PlayerPrefs.GetInt(LOAD_FLAG, 0));
        Debug.Log("Save existe : " + PlayerPrefs.HasKey(SAVE_KEY));

        if (PlayerPrefs.GetInt(LOAD_FLAG, 0) == 1 && PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("Démarrage du chargement...");
            StartCoroutine(LoadAfterAllManagers());
        }
        else
        {
            Debug.Log("Nouvelle partie !");
            PlayerPrefs.DeleteKey(LOAD_FLAG);
            PlayerPrefs.Save();
        }
    }

    IEnumerator LoadAfterAllManagers()
    {
        float timeout = 5f;
        float elapsed = 0f;

        // Attend avec timeout pour éviter une boucle infinie
        while (elapsed < timeout)
        {
            if (SaveManager.Instance != null &&
                Inventory.Instance != null &&
                PlayerCardCollection.Instance != null &&
                DeckBuilderManager.Instance != null &&
                BadDecisionManager.Instance != null &&
                PickedUpItemsTracker.Instance != null)
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= timeout)
        {
            Debug.Log("TIMEOUT : Certains managers manquants !");
            Debug.Log("SaveManager : " + (SaveManager.Instance != null));
            Debug.Log("Inventory : " + (Inventory.Instance != null));
            Debug.Log("PlayerCardCollection : " + (PlayerCardCollection.Instance != null));
            Debug.Log("DeckBuilderManager : " + (DeckBuilderManager.Instance != null));
            Debug.Log("BadDecisionManager : " + (BadDecisionManager.Instance != null));
            Debug.Log("PickedUpItemsTracker : " + (PickedUpItemsTracker.Instance != null));
            yield break;
        }

        yield return new WaitForEndOfFrame();

        Debug.Log("Tous les managers prêts !");
        PlayerPrefs.DeleteKey(LOAD_FLAG);
        PlayerPrefs.Save();

        SaveManager.Instance.LoadGame();
    }
}