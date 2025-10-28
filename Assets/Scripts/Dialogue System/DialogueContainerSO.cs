using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "DialogueContainer", menuName="Dialogue/New Dialogue Container")]
public class DialogueContainerSO : ScriptableObject
{
    public List<DialogueSO> dialogueLines = new List<DialogueSO>();

    public DialogueSO GetDialogueByID(string dialogueID)
    {
        return dialogueLines.Find(d => d.DialogueID == dialogueID);
    }

    public void AddDialogue(DialogueSO dialogue)
    {
        if (!dialogueLines.Contains(dialogue))
        {
            dialogueLines.Add(dialogue);
        }
    }

    public void RemoveDialogue(DialogueSO dialogue)
    {
        dialogueLines.Remove(dialogue);
    }


    public void ClearDialogues()
    {
        dialogueLines.Clear();
    }
}
