using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiplashRedemption : BlockerRedemption
{
    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.blockerObject.SetActive(true);
        Invoke("HideBlocker", this.timeToDisplay);

        StartCoroutine(this.HandleVideoPauses());
    }

    private IEnumerator HandleVideoPauses()
    {
        yield return new WaitForSeconds(3.5f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.9f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.38f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.85f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.3f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.17f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.59f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.61f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.29f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(2.41f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.15f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(2.19f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(1.5f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.61f);
        this.ResumeAffectedVideos();
        yield return new WaitForSeconds(0.85f);
        this.PauseAffectedVideos();
        yield return new WaitForSeconds(1.72f);
        this.ResumeAffectedVideos();
    }
}
