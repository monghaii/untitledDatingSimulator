using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class TextFeed : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI option1;
    [SerializeField] public TextMeshProUGUI option2;
    [SerializeField] public TextMeshProUGUI option3;
    [SerializeField] public TextMeshProUGUI option4;
    [SerializeField] public GameObject survivedScreen;
    [SerializeField] public GameObject failedScreen;
    public GameManager gameManager;
    
    
    
    public DialogueLineSO currentDialogue;
    private int currentLineIndex = 0;

    private void Awake()
    {
        Debug.Log(currentDialogue.dialogueText);
        option1.text = currentDialogue.options[0].optionText;
        option2.text = currentDialogue.options[1].optionText;
        option3.text = currentDialogue.options[2].optionText;
        option4.text = currentDialogue.options[3].optionText;
        dialogueText.text = currentDialogue.dialogueText;
    }

    public void DisplayNextDialogue(int option)
    {
        if (option1.text == "Oh no." &&
            option2.text == "Oh no." &&
            option3.text == "Oh no." &&
            option4.text == "Oh no.")
        {
            // TRIGGER FPS HERE
            Debug.Log("FPS TRIGGERED");
            gameManager.StartFPS();
        }
        else if (currentDialogue.options[option].isWinningChoice)
        {
            Debug.Log("IS WINNING CHOICE");
            survivedScreen.gameObject.SetActive(true);
        }
        else if (currentDialogue.options[option].triggersFPS)
        {
            dialogueText.text = "THAT'S IT!!!!!!";
            option1.text = "Oh no.";
            option2.text = "Oh no.";
            option3.text = "Oh no.";
            option4.text = "Oh no.";
        }
        else if (currentDialogue.options[option].causesRelationship)
        {
            Debug.Log("RELATIONSHIP CAUSED");
            failedScreen.gameObject.SetActive(true);
        }
        else
        {
            // select the next dialogue node
            currentDialogue = currentDialogue.options[option].response;
            dialogueText.text = currentDialogue.dialogueText;
            option1.text = currentDialogue.options[0].optionText;
            option2.text = currentDialogue.options[1].optionText;
            option3.text = currentDialogue.options[2].optionText;
            option4.text = currentDialogue.options[3].optionText;
        }
    }
}