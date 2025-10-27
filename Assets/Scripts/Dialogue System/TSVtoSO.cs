using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split('\t');


            DialogueSO dialogue = ScriptableObject.CreateInstance<DialogueSO>();
            dialogue.DialogueID = columns[0];
            dialogue.CharacterName = columns[1];
            dialogue.CharacterEmotion = columns[2];
            dialogue.CharacterDialogue = columns[3];
            dialogue.nextDialogueLine = columns[4];

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

            string safeFileName = SanitizeFileName(dialogue.DialogueID); // ensure that ID wouldn't be invalid ;p
            string assetPath = Path.Combine(saveFolder, $"{safeFileName}.asset");

            string relativePath = "Assets" + assetPath.Replace(Application.dataPath, "");

            AssetDatabase.CreateAsset(dialogue, relativePath);
            Debug.Log($"Created DialogueSO: {relativePath}");
        }

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
