using UnityEngine;

public class HelpScreen : MonoBehaviour
{
    public void ToggleHelpScreen()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}