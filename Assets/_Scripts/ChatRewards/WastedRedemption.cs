using UnityEngine;

public class WastedRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject wastedVideoObject;    

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        return;

        this.wastedVideoObject.SetActive(true);
        Invoke("DeactivateWasted", 8.0f);
    }

    private void DeactivateWasted()
    {
        this.wastedVideoObject.SetActive(false);
    }
}
