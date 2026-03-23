using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;

    private Coroutine typingCoroutine;
    private Coroutine scaleCoroutineNPC;
    private Coroutine scaleCoroutinePlayer;

    [Header("Portraits")]
    public Image playerPortraitImage;
    public Image npcPortraitImage;

    [Header("Animation Portraits")]
    public float activeSpeakerScale = 1.2f;
    public float inactiveSpeakerScale = 1f;
    public float scaleSpeed = 8f;

    private DialogueData currentData;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentFullLine = "";
    private bool currentDialogueIsBad = false;
    private NPCWithItemDialogue currentNPC;

    public float typingSpeed = 0.05f;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data, NPCWithItemDialogue npc = null)
    {
        if (isDialogueActive) return;

        currentNPC = npc;
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
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
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
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (scaleCoroutineNPC != null) StopCoroutine(scaleCoroutineNPC);
        if (scaleCoroutinePlayer != null) StopCoroutine(scaleCoroutinePlayer);

        if (line.speaker == DialogueLine.Speaker.NPC)
        {
            npcNameText.text = currentData.npcName;
            npcPortraitImage.color = Color.white;
            playerPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            scaleCoroutineNPC = StartCoroutine(AnimateScale(npcPortraitImage.rectTransform, activeSpeakerScale));
            scaleCoroutinePlayer = StartCoroutine(AnimateScale(playerPortraitImage.rectTransform, inactiveSpeakerScale));
        }
        else
        {
            npcNameText.text = "Joueur";
            npcPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            playerPortraitImage.color = Color.white;
            scaleCoroutinePlayer = StartCoroutine(AnimateScale(playerPortraitImage.rectTransform, activeSpeakerScale));
            scaleCoroutineNPC = StartCoroutine(AnimateScale(npcPortraitImage.rectTransform, inactiveSpeakerScale));
        }

        typingCoroutine = StartCoroutine(TypeLine(line.text));
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

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        npcPortraitImage.color = Color.white;
        playerPortraitImage.color = Color.white;
        npcPortraitImage.rectTransform.localScale = Vector3.one;
        playerPortraitImage.rectTransform.localScale = Vector3.one;
        StopAllCoroutines();

        if (currentData != null && currentData.triggersCardGame)
        {
            CardGameManager.Instance.StartCardGame(
                currentData.enemyCardData,
                currentData.playerCardData,
                currentData.cardGameReward,
                currentNPC
            );
        }

        currentData = null;

        if (currentDialogueIsBad)
        {
            BadDecisionManager.Instance.TriggerBadDecision();
            currentDialogueIsBad = false;
        }
    }

    public bool IsActive() { return isDialogueActive; }
}