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
            string chatterName = "TestCabbage" + i.ToString();

            if (ChatManager.instance.chatterDict.ContainsKey(chatterName))
            {
                Destroy(ChatManager.instance.chatterDict[chatterName].gameObject);
                ChatManager.instance.currentActiveChatters.Remove(ChatManager.instance.chatterDict[chatterName]);
                ChatManager.instance.chatterDict.Remove(chatterName);
            }
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
        string chatterName = "TestCabbage" + chatterId.ToString();

        float randomXPosition = Random.Range(ChatManager.instance.spawnBoundaries.bounds.min.x, ChatManager.instance.spawnBoundaries.bounds.max.x);
        Vector3 instantiationPosition = new Vector3(randomXPosition, ChatManager.instance.spawnBoundaries.transform.position.y, 0f);
        GameObject newChatter = Instantiate(ChatManager.instance.cabbageChatterPrefab, instantiationPosition, new Quaternion(), ChatManager.instance.parentChat.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        ChatManager.instance.chatterDict.Add(chatterName, cabbageChatter);
        ChatManager.instance.currentActiveChatters.Add(cabbageChatter);
        ChatManager.instance.chatterQueue.Enqueue(cabbageChatter);

        cabbageChatter.chatterName = chatterName;
        cabbageChatter.DisplayChatMessage(chatterName, "Test Message");
        newChatter.name = chatterName;

        //Update chatter with their last shoot score, if it exists
        //Otherwise, initialize it to 0
        if (ChatManager.instance.chatterScoreHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.shootScore = ChatManager.instance.chatterScoreHistory[cabbageChatter.chatterName];
        }
        else
        {
            ChatManager.instance.chatterScoreHistory.Add(cabbageChatter.chatterName, 0);
        }
    }
}
