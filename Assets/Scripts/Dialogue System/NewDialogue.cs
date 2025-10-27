using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class NewDialogue : EditorWindow
{
    Vector2 scrollPos;

    private string fileName = "";
    private string dialogueID = "";
    private string speakerName = "";
    private string speakerEmotion = "";
    private string speakerDialogue = "";
    private string nextID = "";

    private List<string> optionText = new List<string>();
    private List<string> optionConnections = new List<string>();

    [MenuItem("Utilities/Dialogue/New Dialogue Line")]
    public static void ShowWindow()
    {
        NewDialogue dialogueWND = GetWindow<NewDialogue>();
        dialogueWND.titleContent = new GUIContent("Dialogue Editor");
    }


    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("New Dialogue Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        dialogueID = EditorGUILayout.TextField("Dialogue ID: ", dialogueID);
        speakerName = EditorGUILayout.TextField("Character Name: ", speakerName);
        speakerEmotion = EditorGUILayout.TextField(new GUIContent("Character Emotion: ","e.g., Happy, Sad, Anger, Fear, Disgust, Confused, or Surprise (Temporary)"), speakerEmotion);

        EditorGUILayout.LabelField("Dialogue Line:");
        speakerDialogue = EditorGUILayout.TextArea(speakerDialogue, GUILayout.Height(40));

        nextID = EditorGUILayout.TextField(new GUIContent("Next Dialogue: ", "Input specific Dialogue ID || Leave empty to move to the next (index)."), nextID);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Dialogue Options");

        #region Remove Option

        int optionToRemove = -1;

        for (int i = 0; i < optionText.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($" Dialogue Option {i + 1}");

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                optionToRemove = i;
            }
            EditorGUILayout.EndHorizontal();

            optionText[i] = EditorGUILayout.TextField("Option Text: ", optionText[i]);
            optionConnections[i] = EditorGUILayout.TextField("Connect to Dialogue (ID): ", optionConnections[i]);

            EditorGUILayout.EndVertical();
        }

        if (optionToRemove >= 0)
        {
            optionText.RemoveAt(optionToRemove);
            optionConnections.RemoveAt(optionToRemove);
            Repaint();
        }

        #endregion

        #region Add Option
        if (GUILayout.Button("Add Dialogue Option"))
        {
            optionText.Add("");
            optionConnections.Add("");
        }
        #endregion

        EditorGUILayout.Space();

        fileName = EditorGUILayout.TextField(new GUIContent("File Name: ", "Name of the file you want to save the data into."), fileName);

        if (GUILayout.Button("Create Dialogue"))
        {
            CreateNewDialogue();
        }

        EditorGUILayout.EndScrollView();
    }

    void CreateNewDialogue()
    {
        StringBuilder data = new StringBuilder();

        string filePath = EditorUtility.SaveFilePanel("Save Current Data", Application.dataPath, $"{fileName}", "tsv");


        #region Check if File Exist
        if (string.IsNullOrEmpty(filePath)) return;

        bool fileExist = File.Exists(filePath);

        if (!fileExist)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("DialogueID\tCharacterName\tCharacterEmotion\tCharacterDialogue\tNext Dialogue (ID)\tOption Texts\tConnects To ID");
            }
        }
        #endregion

        if (optionText.Count > 0)
        {
            string allOptions = string.Join("|", optionText);
            string allConnections = string.Join("|", optionConnections);

            data.AppendLine($"{dialogueID}\t{speakerName}\t{speakerEmotion}\t{speakerDialogue}\t{nextID}\t{allOptions}\t{allConnections}");
        }
        else
        {
            data.AppendLine($"{dialogueID}\t{speakerName}\t{speakerEmotion}\t{speakerDialogue}\t{nextID}\t\t");
        }

        File.AppendAllText(filePath, data.ToString(), Encoding.UTF8);

        ClearFields();
    }

    private void ClearFields()
    {
        dialogueID = "";
        speakerName = "";
        speakerEmotion = "";
        speakerDialogue = "";
        nextID = "";
        optionText.Clear();
        optionConnections.Clear();
    }
}