using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTestCabbagesButton : MonoBehaviour
{
    public int numCabbages;

    [SerializeField]
    private ChatGameManager chatGameManager;

    public void SpawnCabbages()
    {
        for (int i = 0; i < this.numCabbages; i++)
        {
            this.SpawnNewTestChatter();
        }
    }

    private void SpawnNewTestChatter()
    {
        string chatterName = ("testcabbage" + TestCabbageManager.GetNewTestCabbageID()).ToLower();

        if (this.chatGameManager.IsPlinkoActive() == false)
        {
            CabbageManager.instance.SpawnNewChatter(chatterName);
        }
        else
        {
            this.chatGameManager.ProcessGameCommand(chatterName, Random.Range(1, GameObject.Find("DropZonesParent").transform.childCount + 1).ToString());
        }
        
    }
}
