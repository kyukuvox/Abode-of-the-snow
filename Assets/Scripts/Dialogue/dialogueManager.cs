using TMPro;
using UnityEngine;
using Ink.Runtime;
using System.Collections; //pour utilisé les varaibles de types Story 


public class dialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText; //accéder aux fichiers ink

    private Story currentStory; //pour voir quel ink file est display

    public bool dialogueIsPlaying { get; private set; } //pour voir si le texte se joue

    private static dialogueManager instance;

    private void Awake()
    {
        if ( instance != null) //Mesure de sécurité si le singleton est double 
        {
            Debug.LogWarning("il y a un dialogueManager de plus dans la scène");
        }

        instance = this;
    }

    public static dialogueManager GetInstance()
    {
        return instance;
    }

    private void Start() //pour commencer le dialogue caché 
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // ne se joue pas si dialogue n'est pas entrain de se jouer 
        if (!dialogueIsPlaying) 
        {
            return;
        }

        // se joue si le joueur clique sur le bon bouton 
        if (Input.GetButtonDown("Interact"))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON) // pour rentrer en mode dialogue
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();  //continue vas appeler la prochaine ligne de dialogue 
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
}
