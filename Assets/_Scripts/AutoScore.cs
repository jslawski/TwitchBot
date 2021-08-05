using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScore : MonoBehaviour
{
    [SerializeField]
    private GameObject debugCanvas;
    [SerializeField]
    private static Transform hoopTransform;
    [SerializeField]
    private InputField targetChatter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F8))
        {
            this.debugCanvas.SetActive(!this.debugCanvas.activeSelf);
        }
    }

    public void AutoScoreChatter()
    {
        if (!ChatManager.shootModeActive)
        {
            return;
        }

        GameObject selectedChatter = GameObject.Find(this.targetChatter.text.ToLower());

        if (selectedChatter == null)
        {
            return;
        }

        selectedChatter.transform.position = new Vector3(AutoScore.hoopTransform.position.x, AutoScore.hoopTransform.position.y, selectedChatter.transform.position.z);
    }

    public void AllAutoScore()
    {
        foreach (CabbageChatter chatter in ChatManager.instance.currentActiveChatters)
        {
            chatter.gameObject.transform.position = new Vector3(AutoScore.hoopTransform.position.x, AutoScore.hoopTransform.position.y, chatter.gameObject.transform.position.z);
        }
    }

    public static void SetHoopTransform(Transform hoop)
    {
        AutoScore.hoopTransform = hoop;
    }
}
