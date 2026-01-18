using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerRedemption : RewardRedemption
{
    [SerializeField]
    protected GameObject blockerObject;

    [SerializeField]
    protected float timeToDisplay;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.blockerObject.SetActive(true);
        Invoke("HideBlocker", this.timeToDisplay);
    }

    public void PauseAffectedVideos()
    {
        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].PauseVideo();
            }
        }
    }

    public void ResumeAffectedVideos()
    {
        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].ResumeVideo();
            }
        }
    }

    protected virtual void HideBlocker()
    {
        this.blockerObject.SetActive(false);
        this.ResumeAffectedVideos();
    }
}
