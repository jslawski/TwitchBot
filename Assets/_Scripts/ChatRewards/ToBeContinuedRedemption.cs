using UnityEngine;

public class ToBeContinuedRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject toBeContinuedVideoObject;    

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.toBeContinuedVideoObject.SetActive(true);
        Invoke("DeactivateToBeContinued", 11.5f);
    }

    private void DeactivateToBeContinued()
    {
        this.toBeContinuedVideoObject.SetActive(false);
    }
}
