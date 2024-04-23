using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrail : MonoBehaviour
{
    public Vector3 hitpoint;
    public float timeNeededToReach;
    
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.move(this.gameObject, hitpoint, timeNeededToReach);
        Destroy(this.gameObject, timeNeededToReach + 0.01f);
    }
}
