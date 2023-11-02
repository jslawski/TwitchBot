using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCabbageManager : MonoBehaviour
{
    public static int nextTestCabbageID = 0;

    [SerializeField]
    private GameObject parentChatObject;

    public static int GetNewTestCabbageID()
    {
        return nextTestCabbageID++;
    }

    public void ClearTestCabbages()
    {
        CabbageChatter[] allCurrentChatters = this.parentChatObject.GetComponentsInChildren<CabbageChatter>();

        for (int i = 0; i < allCurrentChatters.Length; i++)
        {
            if (allCurrentChatters[i].chatterName.Contains("testcabbage") == true)
            {
                Destroy(allCurrentChatters[i].gameObject);
            }
        }

        nextTestCabbageID = 0;
    }
}
