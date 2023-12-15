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
        Debug.LogError("This code was commented out to get other stuff to work.  Go here to fix it.");
        /*
        string chatterName = ("TestCabbage" + TestCabbageManager.GetNewTestCabbageID()).ToLower();

        if (CabbageManager.instance.plinko == false)
        {
            CabbageManager.instance.SpawnNewChatter(chatterName);
        }
        else
        {
            CabbageManager.instance.AttemptPlinkoDrop(chatterName, Random.Range(1, GameObject.Find("DropZonesParent").transform.childCount + 1));
        }
        */
    }
}
