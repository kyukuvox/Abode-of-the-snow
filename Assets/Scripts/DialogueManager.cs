using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;       // ou TMP_Text si tu utilises TextMeshPro
    public Text npcNameText;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentFullLine = "";

    public float typingSpeed = 0.05f; // Vitesse d'apparition des lettres

    void Awake()
    {
        Instance = this;
    }

    // Appelé par le PNJ pour démarrer un dialogue
    public void StartDialogue(DialogueData data)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        currentLines = data.lines;
        currentLineIndex = 0;
        npcNameText.text = data.npcName;
        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine(currentLines[currentLineIndex]));
    }

    // Appelé quand le joueur appuie sur E
    public void OnPressE()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            // Si le texte est en train de s'écrire → l'affiche en entier immédiatement
            StopAllCoroutines();
            dialogueText.text = currentFullLine;
            isTyping = false;
        }
        else
        {
            // Passe à la ligne suivante
            currentLineIndex++;

            if (currentLineIndex < currentLines.Length)
                StartCoroutine(TypeLine(currentLines[currentLineIndex]));
            else
                EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        currentFullLine = line;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentLines = null;
    }

    public bool IsActive() { return isDialogueActive; }
}