using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Animator speakerIcon;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject optionBTNPrefab;

    [Header("Data References")]
    [SerializeField] private List<DialogueSO> dialogueList;

    [SerializeField] private InputReader input;

    private int currentIndex = 0;
    private bool hasChosen = false;
    private List<GameObject> activeOptionBTNS = new List<GameObject>();

    
    void OnEnable() { input.Interact += OnInteract; }
    //void OnDisable() { input.Interact -= OnInteract; }


    private void OnInteract()
    {
        if (hasChosen) return;

        if (speakerName == null || dialogueText == null)
        {
            Debug.LogWarning("TMP references not assigned in inspector! >:(");
            return;
        }

        if (currentIndex >= dialogueList.Count)
        {
            currentIndex = 0; // incase the player interacts again later
            return;
        }

        DisplayCurrentDialogue();
    }

    private void DisplayCurrentDialogue()
    {

        DialogueSO currentDialogue = dialogueList[currentIndex];

        speakerIcon.SetTrigger($"{currentDialogue.CharacterEmotion}");
        speakerName.text = currentDialogue.CharacterName;
        dialogueText.text = currentDialogue.CharacterDialogue;

        if (currentDialogue.options != null && currentDialogue.options.Count > 0)
        {
            ShowDialogueOptions(currentDialogue.options);
        } else
        {
            HideOptions();
            NextDialogue(currentDialogue);
        }
    }

    private void ShowDialogueOptions(List<DialogueOption> options)
    {
        hasChosen = true;
        optionsPanel.SetActive(true);

        ClearBTNS();

        foreach (DialogueOption option in options)
        {
            GameObject optionBTN = Instantiate(optionBTNPrefab, optionsPanel.transform);
            activeOptionBTNS.Add(optionBTN);

            TextMeshProUGUI optionBTNText = optionBTN.GetComponentInChildren<TextMeshProUGUI>();
            if (optionBTNText != null )
            {
                optionBTNText.text = option.OptionText;
            }

            Button btn = optionBTN.GetComponent<Button>();
            if ( btn != null )
            {
                string connectingID = option.connectingDialogueID;
                btn.onClick.AddListener ( () => OnOptionSelected(connectingID));
            }
        }
    }

    private void OnOptionSelected(string connectingDialogueID)
    {
        hasChosen = false;
        HideOptions();

        int nextIndex = dialogueList.FindIndex(dialogue => dialogue.DialogueID == connectingDialogueID);

        if (nextIndex != -1)
        {
            currentIndex = nextIndex;
            DisplayCurrentDialogue();
        }
    }

    private void HideOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        ClearBTNS();
    }

    private void ClearBTNS()
    {
        foreach (GameObject btn in activeOptionBTNS)
        {
            Destroy(btn);
        }
        activeOptionBTNS.Clear();
    }

    private void NextDialogue(DialogueSO currentDialogue)
    {
        if (!string.IsNullOrEmpty(currentDialogue.nextDialogueLine))
        {
            int nextIndex = dialogueList.FindIndex(dialogue => dialogue.DialogueID == currentDialogue.nextDialogueLine);

            if (nextIndex != -1)
            {
                currentIndex = nextIndex;
            }
            else
            {
                currentIndex++;
            }
        }
        else
        {
            currentIndex++;
        }
    }
}

