using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    public Button continueButton;
    public Button playButton;
    public Button quitButton;

    private const string SAVE_KEY = "SaveData";
    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        // Désactive immédiatement tous les boutons dans Awake
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        bool hasSave = PlayerPrefs.HasKey(SAVE_KEY);
        Debug.Log("=== TITLE SCREEN START ===");
        Debug.Log("SAVE_KEY utilisée : " + SAVE_KEY);
        Debug.Log("Save existante : " + hasSave);
        Debug.Log("continueButton ref : " + (continueButton != null ? "OK" : "NULL"));

        // Affiche toutes les clés présentes dans PlayerPrefs
        Debug.Log("Clé SaveData présente : " + PlayerPrefs.HasKey("SaveData"));

        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSave);

        Invoke("EnableButtons", 0.5f);
    }

    void EnableButtons()
    {
        if (playButton != null) playButton.interactable = true;
        if (continueButton != null && PlayerPrefs.HasKey(SAVE_KEY))
            continueButton.interactable = true;
        if (quitButton != null) quitButton.interactable = true;
        Debug.Log("Boutons activés !");
    }

    public void PlayGame()
    {
        Debug.Log("=== PLAY GAME CLIQUÉ ===");
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(LOAD_FLAG);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        Debug.Log("=== CONTINUE GAME CLIQUÉ ===");
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("ERREUR : Aucune save !");
            return;
        }
        PlayerPrefs.SetInt(LOAD_FLAG, 1);
        PlayerPrefs.Save();
        Debug.Log("Flag de chargement posé !");
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}