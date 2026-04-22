using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;

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
    private bool isWaitingForInput = false;
    private string currentFullLine = "";
    private bool currentDialogueIsBad = false;
    private NPCWithItemDialogue currentNPC;

    private Coroutine typingCoroutine;
    private Coroutine scaleCoroutineNPC;
    private Coroutine scaleCoroutinePlayer;

    public float typingSpeed = 0.05f;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data, NPCWithItemDialogue npc = null)
    {
        if (isDialogueActive) return;

        EventSystem.current.SetSelectedGameObject(null);

        currentNPC = npc;
        currentDialogueIsBad = data.isBadDecision;
        isDialogueActive = true;
        isWaitingForInput = false;
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
        if (!isWaitingForInput) return;

        isWaitingForInput = false;
        currentLineIndex++;

        if (currentLineIndex < currentData.lines.Length)
            DisplayLine(currentData.lines[currentLineIndex]);
        else
            EndDialogue();
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
        isWaitingForInput = false;
        currentFullLine = line;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isWaitingForInput = true;
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
        isWaitingForInput = false;
        dialoguePanel.SetActive(false);
        npcPortraitImage.color = Color.white;
        playerPortraitImage.color = Color.white;
        npcPortraitImage.rectTransform.localScale = Vector3.one;
        playerPortraitImage.rectTransform.localScale = Vector3.one;
        StopAllCoroutines();

        bool npcAlreadyDefeated = currentNPC != null && currentNPC.HasBeenDefeated();

        Debug.Log("=== END DIALOGUE ===");
        Debug.Log("triggersCardGame : " + (currentData != null ? currentData.triggersCardGame.ToString() : "currentData NULL"));
        Debug.Log("npcAlreadyDefeated : " + npcAlreadyDefeated);
        Debug.Log("CardGameManager : " + (CardGameManager.Instance != null ? "OK" : "NULL"));
        Debug.Log("enemyCardData : " + (currentData != null && currentData.enemyCardData != null ? "OK" : "NULL"));
        Debug.Log("playerCardData : " + (currentData != null && currentData.playerCardData != null ? "OK" : "NULL"));

        if (currentData != null && currentData.triggersCardGame && !npcAlreadyDefeated)
        {
            CardGameManager.Instance.StartCardGame(
                currentData.enemyCardData,
                currentData.playerCardData,
               currentData.cardGameRewards,
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
    public bool IsWaitingForInput() { return isWaitingForInput; }
}