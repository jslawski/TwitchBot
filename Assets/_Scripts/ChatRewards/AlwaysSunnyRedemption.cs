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
        Invoke("DeactivateAlwaysSunny", 7.5f);
    }

    private void DeactivateAlwaysSunny()
    {
        this.alwaysSunnyPanel.SetActive(false);
    }
}
