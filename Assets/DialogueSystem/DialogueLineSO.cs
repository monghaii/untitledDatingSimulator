using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue System/Dialogue Line")]
public class DialogueLineSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string dialogueText;
    
    public DialogueOption[] options = new DialogueOption[4];
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueLineSO response;
    public bool triggersFPS;
    public bool isWinningChoice;
    public bool causesRelationship;
}