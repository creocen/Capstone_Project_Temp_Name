using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TSVtoSO
{
    [MenuItem("Utilities/Dialogue/Import Dialogue")]
    public static void ImportDialogue()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Dialogue File (.tsv)", Application.dataPath, "tsv");
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("No TSV file selected.");
            return;
        }

        string saveFolder = EditorUtility.OpenFolderPanel("Select Folder to Save Dialogue Outputs", Application.dataPath, "");
        if (string.IsNullOrEmpty(saveFolder))
        {
            Debug.LogWarning("No output folder selected.");
            return;
        }

        DialogueContainerSO container = GetOrCreateContainer(saveFolder);
        if (container == null)
        {
            Debug.LogWarning("No Dialogue Container selected or created.");
            return;
        }
        

        string[] lines = File.ReadAllLines(filePath);
        List<DialogueSO> importedDialogues = new List<DialogueSO>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split('\t');


            /*DialogueSO dialogue = ScriptableObject.CreateInstance<DialogueSO>();
            dialogue.DialogueID = columns[0];
            dialogue.CharacterName = columns[1];
            dialogue.CharacterEmotion = columns[2];
            dialogue.CharacterDialogue = columns[3];
            dialogue.nextDialogueLine = columns[4];*/

            string dialogueID = columns[0];
            string fileName = SanitizeFileName(dialogueID);
            string assetPath = Path.Combine(saveFolder, $"{fileName}.asset");
            string relativePath = "Assets" + assetPath.Replace(Application.dataPath, "");

            // Check if DialogueSO already exists
            DialogueSO dialogue = AssetDatabase.LoadAssetAtPath<DialogueSO>(relativePath);
            if (dialogue == null)
            {
                dialogue = ScriptableObject.CreateInstance<DialogueSO>();
                AssetDatabase.CreateAsset(dialogue, relativePath);
                Debug.Log($"Created new dialogueSO: {assetPath}");
            } 
            else
            {
                Debug.Log($"Updated existing dialogueSO: {relativePath}");
            }

            dialogue.DialogueID = columns[0];
            dialogue.CharacterName = columns[1];
            dialogue.CharacterEmotion = columns[2];
            dialogue.CharacterDialogue = columns[3];
            dialogue.nextDialogueLine = columns[4];

            dialogue.options.Clear();

            string[] optionTexts = string.IsNullOrEmpty(columns[5]) ? new string[0] : columns[5].Split('|');
            string[] connections = string.IsNullOrEmpty(columns[6]) ? new string[0] : columns[6].Split('|');

            for (int j = 0; j < optionTexts.Length; j++)
            {
                DialogueOption option = new DialogueOption
                {
                    OptionText = optionTexts[j],
                    connectingDialogueID = j < connections.Length ? connections[j] : ""
                };
                dialogue.options.Add(option);
            }

            /*string safeFileName = SanitizeFileName(dialogue.DialogueID); // ensure that ID wouldn't be invalid ;p
            string assetPath = Path.Combine(saveFolder, $"{safeFileName}.asset");

            string relativePath = "Assets" + assetPath.Replace(Application.dataPath, "");

            AssetDatabase.CreateAsset(dialogue, relativePath);
            Debug.Log($"Created DialogueSO: {relativePath}");*/

            EditorUtility.SetDirty(dialogue);
            importedDialogues.Add(dialogue);

            
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        UpdateContainer(container, importedDialogues);
        Debug.Log($"Successfully imported {importedDialogues.Count} dialogue lines into container: {AssetDatabase.GetAssetPath(container)}");

    }

    private static DialogueContainerSO GetOrCreateContainer(string saveFolder)
    {
        string containerPath = Path.Combine(saveFolder, "DialogueContainer.asset");
        string relativeContainerPath = "Assets" + containerPath.Replace(Application.dataPath, "");

        DialogueContainerSO container = AssetDatabase.LoadAssetAtPath<DialogueContainerSO>(relativeContainerPath);
        if (container == null)
        {
            // new container or select an existing one
            int choice = EditorUtility.DisplayDialogComplex(
                "Dialogue Container",
                "No Dialogue Container found in this folder. Would you like to create a new one or select an existing one?",
                "Create New",
                "Cancel",
                "Select Existing"
            );

            // new container
            if (choice == 0)
            {
                container = ScriptableObject.CreateInstance<DialogueContainerSO>();
                AssetDatabase.CreateAsset(container, relativeContainerPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"Created new DialogueContainer at: {relativeContainerPath}");
            } 
            else if( choice == 2 ) 
            {
                // if existing
                string existingPath = EditorUtility.OpenFilePanel("Select Dialogue Container", Application.dataPath, "asset");
                if(!string.IsNullOrEmpty(existingPath))
                {
                    string relativeExistingPath = "Assets" + existingPath.Replace(Application.dataPath, "");
                    container = AssetDatabase.LoadAssetAtPath<DialogueContainerSO>(relativeExistingPath);
                }
            }
        }
        return container;
    }

    private static void UpdateContainer(DialogueContainerSO container, List<DialogueSO> importedDialogues)
    {
        // remove null refs
        container.dialogueLines.RemoveAll(d => d == null);

        foreach (DialogueSO dialogue in importedDialogues)
        {
            if (!container.dialogueLines.Contains(dialogue))
            {
                container.dialogueLines.Add(dialogue);
            }
        }

        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssetIfDirty(container);

        Debug.Log($"Container now has {container.dialogueLines.Count} dialogue lines");
    }


    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c.ToString(), "_");

        }
        return name;
    }
}


