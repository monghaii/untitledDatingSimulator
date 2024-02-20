using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacters", menuName = "Characters/Characters")]
public class Characters : ScriptableObject
{
    [SerializeField]
    private List<Character> characterList = new List<Character>();

    public List<Character> CharacterList
    {
        get { return characterList; }
        set { characterList = value; }
    }
}

[System.Serializable]
public class Character
{
    [SerializeField]
    private string characterName;
    
    [SerializeField]
    private Sprite characterImage;

    public string CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }

    public Sprite CharacterImage
    {
        get { return characterImage; }
        set { characterImage = value; }
    }
}