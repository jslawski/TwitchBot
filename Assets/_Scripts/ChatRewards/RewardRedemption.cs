using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RewardRedemption : MonoBehaviour
{
    // Update is called once per frame
    public abstract void TriggerReward(string userRedeemed, string redemptionMessage = "");
}
