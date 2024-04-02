using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    public enum TutorialStep
    {
        NotStarted = -1,
        Movement = 0,
        Shoot = 1,
        Reload = 2,
        Crouch = 3,
        Sprint = 4,
    }
    private TutorialStep curStep = TutorialStep.NotStarted;
    private Dictionary<KeyCode, bool> inputDetected;
    private bool queuedInvoke = false;
    public TextMeshProUGUI Movement;
    public TextMeshProUGUI Shoot;
    public TextMeshProUGUI Reload;
    public TextMeshProUGUI Crouch;
    public TextMeshProUGUI CrouchHint;
    public TextMeshProUGUI Sprint;
    public Color currTarget;
    public Color Pass;
    public Color NotPassed;
    public GameObject[] slides;
    private int currSlideIdx; 

    void Start()
    {
        inputDetected = new Dictionary<KeyCode, bool>();
        Movement.color = currTarget;
        Shoot.color = NotPassed;
        Reload.color = NotPassed;
        Crouch.color = NotPassed;
        Sprint.color = NotPassed;
        CrouchHint.color = NotPassed;
        if(slides.Length > 0)
        {
            currSlideIdx = 0;
            Invoke("NextSlide", 4.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("cur step: " + curStep);
        switch (curStep)
        {
            case TutorialStep.Movement:
                //slides[currSlideIdx++].SetActive(true);
                if (Input.GetKeyDown(KeyCode.W))
                {
                    inputDetected[KeyCode.W] = true;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    inputDetected[KeyCode.A] = true;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    inputDetected[KeyCode.S] = true;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    inputDetected[KeyCode.D] = true;
                }
                if (inputDetected[KeyCode.W] && inputDetected[KeyCode.A] && inputDetected[KeyCode.S] && inputDetected[KeyCode.D])
                {
                    Movement.color = Pass;
                    if (!queuedInvoke)
                    {
                        queuedInvoke = true;
                        Invoke("NextStep", 1.0f);
                    }

                }
                break;
            case TutorialStep.Shoot:
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot.color = Pass;
                    if (!queuedInvoke)
                    {
                        queuedInvoke = true;
                        Invoke("NextStep", 1.0f);
                    }
                }
                else if (!queuedInvoke)
                {
                    Shoot.color = currTarget;
                }
                break;
            case TutorialStep.Reload:
              
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Reload.color = Pass;
                    if (!queuedInvoke)
                    {
                        queuedInvoke = true;
                        Invoke("NextStep", 1.0f);
                    }
                }
                else if (!queuedInvoke)
                {
                    Reload.color = currTarget;
                }
                break;
            case TutorialStep.Crouch:
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    Crouch.color = Pass;
                    CrouchHint.color = Pass;
                    if (!queuedInvoke)
                    {
                        queuedInvoke = true;
                        Invoke("NextStep", 1.0f);
                    }
                }
                else if (!queuedInvoke)
                {
                    Crouch.color = currTarget;
                    CrouchHint.color = currTarget;
                }
                break;
            case TutorialStep.Sprint:
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Sprint.color = Pass;
                    if (!queuedInvoke)
                    {
                        queuedInvoke = true;
                        Invoke("EndGameUI", 2.0f);
                    }
                }
                else if (!queuedInvoke)
                {
                    Sprint.color = currTarget;
                }
                break;
        }
    }

    void NextSlide()
    {
        if (currSlideIdx <= slides.Length - 3)
        {
            slides[currSlideIdx].SetActive(false);
            currSlideIdx++;
            slides[currSlideIdx].SetActive(true);
            if(currSlideIdx == slides.Length - 2)
            {
                curStep++;
            }
            Invoke("NextSlide", 4.0f);
        }
    }

    void NextStep()
    {
        curStep++;
        queuedInvoke = false;
    }

    void EndGameUI()
    {
        slides[currSlideIdx].SetActive(false);
        slides[slides.Length-1].SetActive(true);
    }
}
