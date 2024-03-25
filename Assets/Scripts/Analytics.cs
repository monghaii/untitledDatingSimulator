using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

public class Analytics : MonoBehaviour
{
    // Method to log an analytic event
    public static void LogAnalyticEvent(string eventName, Value properties = null)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        string debugString = "// ANALYTICS: " + eventName;
        if (properties != null)
        {
            debugString = debugString + " props: " + properties.ToString();
        }
        
        Debug.Log(debugString);
        
        if (gameManager != null)
        {
            // Do not log in dev mode
            if (gameManager.FLAG_DEV_DISABLE_LOGGING)
            {
                Debug.Log("Logging is disabled in development mode.");
                return;
            }
        }
        else
        {
            Debug.LogWarning("GameManager not found in the scene.");
        }

        // Track the event with event name and properties
        Mixpanel.Track(eventName, properties);
    }
}