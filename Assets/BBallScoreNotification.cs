using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallScoreNotification : MonoBehaviour
{
    public delegate void OnScoreDetected(string username, int score);
    public static OnScoreDetected DisplayScoreNotification;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
