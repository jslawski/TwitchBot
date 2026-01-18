using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiiMenuRedemption : BlockerRedemption
{
    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.blockerObject.SetActive(true);
        Invoke("HideBlocker", this.timeToDisplay);

        this.PauseAffectedVideos();
    }
}
