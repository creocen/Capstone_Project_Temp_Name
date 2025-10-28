using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class NewTSVFile
{
    [MenuItem("Utilities/Dialogue/New TSV File")]
    public static void CreateTSVFile()
    {
        string filePath = EditorUtility.SaveFilePanel("Save TSV File", Application.dataPath, "New_TSV_File (Rename)", "tsv");
        
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("DialogueID\tCharacterName\tCharacterEmotion\tCharacterDialogue\tNext Dialogue (ID)\tOption Texts\tConnects To ID");
            }
            Debug.Log($"TSV file created successfully at: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create TSV file: {e.Message}");
        }
    }
}
