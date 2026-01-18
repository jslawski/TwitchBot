using UnityEngine;
using UnityEngine.UI;

public class AlwaysSunnyRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject alwaysSunnyPanel;
    [SerializeField]
    private Text alwaysSunnyText;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.alwaysSunnyText.text = "\"" + redemptionMessage + "\"";
        this.alwaysSunnyPanel.SetActive(true);

        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].PauseVideo();
            }
        }

        Invoke("DeactivateAlwaysSunny", 7.5f);
    }

    private void DeactivateAlwaysSunny()
    {
        this.alwaysSunnyPanel.SetActive(false);

        for (int i = 0; i < this.affectedVideoRewards.Length; i++)
        {
            if (this.affectedVideoRewards[i].gameObject.activeSelf == true)
            {
                this.affectedVideoRewards[i].ResumeVideo();
            }
        }
    }
}
