public class NukeRedemption : RewardRedemption
{
    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        string sanitizedTarget = redemptionMessage.Trim();
        if (redemptionMessage.Contains("@"))
        {
            sanitizedTarget = redemptionMessage.Substring(1).ToLower();
        }

        sanitizedTarget = sanitizedTarget.ToLower();

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
