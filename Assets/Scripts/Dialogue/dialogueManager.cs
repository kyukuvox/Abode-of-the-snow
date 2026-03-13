using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Ink")]
    public TextAsset inkFile; // glisse le .json compilé ici

    [Header("UI")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button[] choiceButtons;

    private Story _story;

    void Start()
    {
        dialoguePanel.SetActive(false); // cache le panel au démarrage
    }

    void Awake() => Instance = this;

    public void StartDialogue(string itemId)
    {
        _story = new Story(inkFile.text);
        _story.variablesState["item_name"] = itemId;

        dialoguePanel.SetActive(true);
        _story.ChoosePathString("present_item");
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (_story.canContinue)
        {
            dialogueText.text = _story.Continue();
            ShowChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowChoices()
    {
        foreach (var btn in choiceButtons)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < _story.currentChoices.Count; i++)
        {
            int index = i;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<Text>().text = _story.currentChoices[i].text;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => ChooseOption(index));
        }
    }

    void ChooseOption(int index)
    {
        _story.ChooseChoiceIndex(index);
        DisplayNextLine();
    }

    void EndDialogue() => dialoguePanel.SetActive(false);
}
