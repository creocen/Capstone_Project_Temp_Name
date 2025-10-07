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

    [Header("Data References")]
    [SerializeField] private List<DialogueSO> dialogueList;

    [SerializeField] private InputReader input;
    private int currentIndex = 0;

    
    void OnEnable() { input.Interact += OnInteract; }
    //void OnDisable() { input.Interact -= OnInteract; }


    private void OnInteract()
    {
        if (speakerName == null || dialogueText == null)
        {
            Debug.LogWarning("TMP references not assigned in inspector!");
            return;
        }

        if (currentIndex >= dialogueList.Count)
        {
            currentIndex = 0; // incase the player interacts again later
            return;
        }

        DialogueSO currentDialogue = dialogueList[currentIndex];

        speakerIcon.SetTrigger($"{currentDialogue.CharacterEmotion}");
        speakerName.text = currentDialogue.CharacterName;
        dialogueText.text = currentDialogue.CharacterDialogue;

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

