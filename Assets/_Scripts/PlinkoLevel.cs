using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoLevel : MonoBehaviour
{
    private DropZone[] dropZones;

    [SerializeField]
    private GameObject cabbagePlinkoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        this.dropZones = GameObject.Find("DropZonesParent").GetComponentsInChildren<DropZone>();
    }

    public void ProcessDropCommand(string username, int dropNum)
    {
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (dropNum == dropZones[i].dropIndex)
            {
                GameObject newCabbage = Instantiate(cabbagePlinkoPrefab, Vector3.zero, new Quaternion(), ChatManager.instance.parentPlinko.transform);
                CabbagePlinko cabbagePlinko = newCabbage.GetComponent<CabbagePlinko>();
                cabbagePlinko.chatterName = username;
                cabbagePlinko.name = username;

                dropZones[i].DropCabbage(newCabbage);

                //Update chatter with their last shoot score, if it exists
                //Otherwise, initialize it to 0
                if (ChatManager.instance.chatterScoreHistory.ContainsKey(username))
                {
                    cabbagePlinko.shootScore = ChatManager.instance.chatterScoreHistory[username];

                    //Toggle crown if the leader has respawned
                    if (Leaderboard.instance.topLeaders[0].username.text == cabbagePlinko.username.text)
                    {
                        cabbagePlinko.ActivateCrown();
                    }
                }
                else
                {
                    ChatManager.instance.chatterScoreHistory.Add(cabbagePlinko.chatterName, 0);
                }

                //Do the same thing with prestige
                if (ChatManager.instance.chatterPrestigeHistory.ContainsKey(cabbagePlinko.chatterName))
                {
                    cabbagePlinko.prestigeLevel = ChatManager.instance.chatterPrestigeHistory[cabbagePlinko.chatterName];

                    //Toggle prestige badge if player has one
                    if (cabbagePlinko.prestigeLevel > 0)
                    {
                        cabbagePlinko.UpdatePrestigeBadge();
                    }
                }
                else
                {
                    ChatManager.instance.chatterPrestigeHistory.Add(cabbagePlinko.chatterName, 0);
                }
            }
        }
    }
}
