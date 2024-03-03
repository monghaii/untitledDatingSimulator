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
    private Sprite[] characterImage;

    [SerializeField]
    public float characterAffection = 50.0f;
    private float defaultAffection = 50.0f;

    public void InitializeCharacter()
    {
        characterAffection = defaultAffection;
    }

    public string CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }

    public Sprite CharacterImage(int index)
    {
        return characterImage[index];
    }

    public void ChangeAffection(float amount)
    {
        characterAffection += amount;
    }
    
    public void SetAffection(float val)
    {
        characterAffection = val;
    }
}