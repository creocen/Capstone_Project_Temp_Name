using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueSO : ScriptableObject
{
    public string DialogueID;
    public string CharacterName;
    public string CharacterEmotion;
    public string CharacterDialogue;

    public List<DialogueOption> options = new List<DialogueOption>();
    public string nextDialogueLine; // if empty moves to next index

}

[System.Serializable]
public class DialogueOption
{
    public string OptionText;
    public string connectingDialogueID;
    //public bool meetsConditions = true;
}
