using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchZone : MonoBehaviour
{
    [SerializeField]
    private int catchPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cabbage")
        {
            CabbageChatter scorer = other.gameObject.GetComponent<CabbageChatter>();
            this.AwardPoints(scorer);
            ChatManager.instance.RemoveCabbage(scorer.chatterName);
        }
    }

    private void AwardPoints(CabbageChatter scorer)
    {
        scorer.shootScore += catchPoints;

        while (scorer.shootScore >= 10)
        {
            scorer.TriggerPrestige();
        }

        ChatManager.instance.chatterScoreHistory[scorer.chatterName.ToLower()] = scorer.shootScore;
        ChatManager.instance.chatterPrestigeHistory[scorer.chatterName.ToLower()] = scorer.prestigeLevel;
        Leaderboard.instance.UpdateLeaderboard(scorer);
    }
}
