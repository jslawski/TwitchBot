using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTestCabbagesButton : MonoBehaviour
{
    public int numCabbages;

    public void SpawnCabbages()
    {
        for (int i = 0; i < this.numCabbages; i++)
        {
            this.SpawnNewTestChatter();
        }
    }

    private void SpawnNewTestChatter()
    {
        string chatterName = ("TestCabbage" + TestCabbageManager.GetNewTestCabbageID()).ToLower();

        if (ChatManager.instance.plinko == false)
        {
            ChatManager.instance.SpawnNewChatter(chatterName);
        }
        else
        {
            ChatManager.instance.AttemptPlinkoDrop(chatterName, Random.Range(1, GameObject.Find("DropZonesParent").transform.childCount + 1));
        }
    }
}
