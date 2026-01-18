using UnityEngine;

public class ToBeContinuedRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject toBeContinuedVideoObject;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.toBeContinuedVideoObject.SetActive(true);
        Invoke("PauseAffectedRewards", 3.8f);
        Invoke("DeactivateToBeContinued", 11.5f);
    }

    private void PauseAffectedRewards()
    {
        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].PauseVideo();
            }
        }
    }

    private void DeactivateToBeContinued()
    {
        this.toBeContinuedVideoObject.SetActive(false);

        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].ResumeVideo();
            }
        }
    }
}
