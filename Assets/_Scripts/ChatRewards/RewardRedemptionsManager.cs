using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.PubSub.Events;

public class RewardRedemptionsManager : MonoBehaviour
{
    private PubSub pubSubClient;

    private Dictionary<string, RewardRedemption> rewardsDict;
    
    private void Awake()
    {
        this.CreateRewardsDict();

        this.pubSubClient = new PubSub();
        this.pubSubClient.OnPubSubServiceConnected += this.PubSubConnected;
        this.pubSubClient.OnRewardRedeemed += this.PubSubRewardRedeemed;
        this.pubSubClient.Disconnect();
        this.pubSubClient.Connect();
    }

    private void CreateRewardsDict()
    {
        RewardRedemption[] rewards = GetComponentsInChildren<RewardRedemption>();

        if (rewards.Length <= 0)
        {
            return;
        }

        this.rewardsDict = new Dictionary<string, RewardRedemption>();

        this.rewardsDict.Add(SecretKeys.ShotsRewardID, rewards[0].GetComponent<RewardRedemption>());
        this.rewardsDict.Add(SecretKeys.AlwaysSunnyRewardID, rewards[1].GetComponent<RewardRedemption>());
        this.rewardsDict.Add(SecretKeys.NukeCabbageRewardID, rewards[2].GetComponent<RewardRedemption>());
        this.rewardsDict.Add(SecretKeys.AchievementUnlockedID, rewards[3].GetComponent<RewardRedemption>());
    }

    private void PubSubConnected(object sender, System.EventArgs e)
    {
        pubSubClient.ListenToRewards(SecretKeys.ChannelID);
        pubSubClient.SendTopics();
    }

    private void PubSubRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Debug.Log("Reward ID: " + e.RewardId.ToString());

        this.rewardsDict[e.RewardId.ToString()].TriggerReward(e.DisplayName, e.Message);
    }
}
