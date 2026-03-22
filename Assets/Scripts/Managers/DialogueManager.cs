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

    public float activeSpeakerScale = 1.2f; 
    public float inactiveSpeakerScale = 1f; 
    public float scaleSpeed = 8f;

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

        npcPortraitImage.sprite = currentData.npcPortrait;
        playerPortraitImage.sprite = currentData.playerPortrait;
        npcPortraitImage.gameObject.SetActive(true);
        playerPortraitImage.gameObject.SetActive(true);

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
            npcPortraitImage.color = Color.white;
            playerPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);

            StopCoroutine("AnimateScale");
            StartCoroutine(AnimateScale(npcPortraitImage.rectTransform, activeSpeakerScale));
            StartCoroutine(AnimateScale(playerPortraitImage.rectTransform, inactiveSpeakerScale));
        }
        else
        {
            npcNameText.text = "Joueur";
            npcPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            playerPortraitImage.color = Color.white;

            StartCoroutine(AnimateScale(playerPortraitImage.rectTransform, activeSpeakerScale));
            StartCoroutine(AnimateScale(npcPortraitImage.rectTransform, inactiveSpeakerScale));
        }

        StartCoroutine(TypeLine(line.text));
    }

    IEnumerator AnimateScale(RectTransform target, float targetScale)
    {
        Vector3 destination = Vector3.one * targetScale;

        while (Vector3.Distance(target.localScale, destination) > 0.01f)
        {
            target.localScale = Vector3.Lerp(
                target.localScale,
                destination,
                Time.deltaTime * scaleSpeed
            );
            yield return null;
        }

        target.localScale = destination;
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

        npcPortraitImage.color = Color.white;
        playerPortraitImage.color = Color.white;

        if (currentDialogueIsBad)
        {
            BadDecisionManager.Instance.TriggerBadDecision();
            currentDialogueIsBad = false;
        }
    }

    public bool IsActive() { return isDialogueActive; }
}