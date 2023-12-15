using UnityEngine;

public class ShotsRedemption : RewardRedemption
{
    [SerializeField]
    private ParticleSystem shotsHype;
    [SerializeField]
    private AudioSource shotsAudio;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.shotsHype.Play();
        this.shotsAudio.Play();
    }    
}
