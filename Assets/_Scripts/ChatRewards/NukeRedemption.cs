
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.PubSub.Events;

public class NukeRedemption : RewardRedemption
{
    [SerializeField]
    private RenderTexture deathRenderTexture;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.deathRenderTexture.Release();

        string sanitizedTarget = redemptionMessage.Trim();        
        if (redemptionMessage.Contains("@"))
        {
            sanitizedTarget = redemptionMessage.Substring(1).ToLower();
        }

        sanitizedTarget = sanitizedTarget.Trim().ToLower();

        Debug.LogError(sanitizedTarget);

        if (CabbageManager.instance.DoesChatterExist(sanitizedTarget))
        {
            CabbageManager.instance.GetCabbageChatter(sanitizedTarget).NukeCabbage();

            if (userRedeemed == sanitizedTarget)
            {
                CabbageManager.instance.SendBotMessage(userRedeemed + " nuked themselves!");
            }
            else
            {
                CabbageManager.instance.SendBotMessage(userRedeemed + " nuked " + sanitizedTarget);
            }
        }
    }
}
