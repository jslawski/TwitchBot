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

            if (ChatManager.instance.chatterDict.ContainsKey(chatterName.ToLower()))
            {
                Destroy(ChatManager.instance.chatterDict[chatterName.ToLower()].gameObject);
                ChatManager.instance.currentActiveChatters.Remove(ChatManager.instance.chatterDict[chatterName.ToLower()]);
                ChatManager.instance.chatterDict.Remove(chatterName.ToLower());
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
        string chatterName = ("TestCabbage" + chatterId.ToString()).ToLower();

        float randomXPosition = Random.Range(ChatManager.instance.spawnBoundaries.bounds.min.x, ChatManager.instance.spawnBoundaries.bounds.max.x);
        Vector3 instantiationPosition = new Vector3(randomXPosition, ChatManager.instance.spawnBoundaries.transform.position.y, 0f);
        GameObject newChatter = Instantiate(ChatManager.instance.cabbageChatterPrefab, instantiationPosition, new Quaternion(), ChatManager.instance.parentChat.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        ChatManager.instance.chatterDict.Add(chatterName.ToLower(), cabbageChatter);
        ChatManager.instance.currentActiveChatters.Add(cabbageChatter);
        ChatManager.instance.chatterQueue.Enqueue(cabbageChatter);

        cabbageChatter.chatterName = chatterName;
        cabbageChatter.DisplayChatMessage(chatterName, "Test Message");
        newChatter.name = chatterName;

        //Update chatter with their last shoot score, if it exists
        //Otherwise, initialize it to 0
        if (ChatManager.instance.chatterScoreHistory.ContainsKey(cabbageChatter.chatterName.ToLower()))
        {
            cabbageChatter.shootScore = ChatManager.instance.chatterScoreHistory[cabbageChatter.chatterName.ToLower()];

            //Toggle crown if the leader has respawned
            if (Leaderboard.instance.topLeaders[0].username.text == cabbageChatter.chatterName.ToLower())
            {
                cabbageChatter.ActivateCrown();
            }
        }
        else
        {
            ChatManager.instance.chatterScoreHistory.Add(cabbageChatter.chatterName.ToLower(), 0);
        }

        //Do the same thing with prestige
        if (ChatManager.instance.chatterPrestigeHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.prestigeLevel = ChatManager.instance.chatterPrestigeHistory[cabbageChatter.chatterName];

            //Toggle prestige badge if player has one
            if (cabbageChatter.prestigeLevel > 0)
            {
                cabbageChatter.UpdatePrestigeBadge();
            }
        }
        else
        {
            ChatManager.instance.chatterPrestigeHistory.Add(cabbageChatter.chatterName, 0);
        }
    }
}
