using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;

    [Header("Portraits")]
    public Image playerPortraitImage;
    public Image npcPortraitImage;

    private DialogueData currentData;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentFullLine = "";
    private bool currentDialogueIsBad = false;

    public float typingSpeed = 0.05f;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        if (isDialogueActive) return;

        currentDialogueIsBad = data.isBadDecision;

        isDialogueActive = true;
        currentData = data;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);

        DisplayLine(currentData.lines[currentLineIndex]);
    }

    public void OnPressE()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentFullLine;
            isTyping = false;
        }
        else
        {
            currentLineIndex++;

            if (currentLineIndex < currentData.lines.Length)
                DisplayLine(currentData.lines[currentLineIndex]);
            else
                EndDialogue();
        }
    }

    void DisplayLine(DialogueLine line)
    {
        if (line.speaker == DialogueLine.Speaker.NPC)
        {
            npcNameText.text = currentData.npcName;
            npcPortraitImage.sprite = currentData.npcPortrait;
            npcPortraitImage.gameObject.SetActive(true);
            playerPortraitImage.gameObject.SetActive(false);
        }
        else
        {
            npcNameText.text = "Joueur";
            playerPortraitImage.sprite = currentData.playerPortrait;
            playerPortraitImage.gameObject.SetActive(true);
            npcPortraitImage.gameObject.SetActive(false);
        }

        StartCoroutine(TypeLine(line.text));
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

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentData = null;
        StopAllCoroutines();

        if (currentDialogueIsBad)
        {
            BadDecisionManager.Instance.TriggerBadDecision();
            currentDialogueIsBad = false;
        }
    }

    public bool IsActive() { return isDialogueActive; }
}