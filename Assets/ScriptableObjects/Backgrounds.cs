using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBackgrounds", menuName = "Backgrounds/Backgrounds")]
public class Backgrounds : ScriptableObject
{
    [SerializeField]
    private List<Background> backgroundList = new List<Background>();

    public List<Background> BackgroundList
    {
        get { return backgroundList; }
        set { backgroundList = value; }
    }
}

[System.Serializable]
public class Background
{
    [SerializeField]
    private string backgroundName;

    [SerializeField]
    private Sprite backgroundImage;

    public string BackgroundName
    {
        get { return backgroundName; }
        set { backgroundName = value; }
    }

    public Sprite BackgroundImage
    {
        get { return backgroundImage; }
        set { backgroundImage = value; }
    }
}