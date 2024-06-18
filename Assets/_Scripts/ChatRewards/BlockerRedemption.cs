using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject blockerObject;

    [SerializeField]
    private float timeToDisplay;
    
    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.blockerObject.SetActive(true);
        Invoke("HideBlocker", this.timeToDisplay);
    }

    private void HideBlocker()
    {
        this.blockerObject.SetActive(false);
    }
}
