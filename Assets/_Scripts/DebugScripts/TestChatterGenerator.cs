using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChatterGenerator : MonoBehaviour
{
    public int numTestCabbagesToSpawn = 10;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F10))
        {
            this.ClearTestCabbages();
            this.SpawnTestChatterBatch();
        }
        if (Input.GetKeyUp(KeyCode.F9))
        {
            this.ClearTestCabbages();
        }
    }

    private void ClearTestCabbages()
    {
        for (int i = 0; i < numTestCabbagesToSpawn; i++)
        {
            string chatterName = ("TestCabbage" + i.ToString()).ToLower();

            CabbageManager.instance.RemoveCabbage(chatterName);
        }
    }

    private void SpawnTestChatterBatch()
    {
        for (int i = 0; i < this.numTestCabbagesToSpawn; i++)
        {
            this.SpawnNewTestChatter(i);
        }
    }

    private void SpawnNewTestChatter(int chatterId)
    {
        Debug.LogError("This code was commented out to get other stuff to work.  Go here to fix it.");        
        /*
        string chatterName = ("TestCabbage" + chatterId.ToString()).ToLower();

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
