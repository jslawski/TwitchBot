using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatchZone : MonoBehaviour
{
    [SerializeField]
    private int catchPoints;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private AudioSource catchAudio;

    private void Awake()
    {
        this.pointsText.text = catchPoints.ToString();

        if (catchPoints == 0)
        {
            this.pointsText.text = "New Lvl";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cabbage")
        {
            CabbageChatter scorer = other.gameObject.GetComponent<CabbageChatter>();
            this.AwardPoints(scorer);
            ChatManager.instance.RemoveCabbage(scorer.chatterName);
        }
    }

    private void AwardPoints(CabbageChatter scorer)
    {
        if (catchPoints == 0)
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(this.LoadNextLevel());
            return;
        }

        scorer.shootScore += catchPoints;

        while (scorer.shootScore >= ChatManager.instance.prestigeThreshold)
        {
            scorer.TriggerPrestige();
        }

        ChatManager.instance.chatterScoreHistory[scorer.chatterName.ToLower()] = scorer.shootScore;
        ChatManager.instance.chatterPrestigeHistory[scorer.chatterName.ToLower()] = scorer.prestigeLevel;
        Leaderboard.instance.UpdateLeaderboard(scorer);

        if (catchPoints > 3)
        {
            this.catchAudio.clip = Resources.Load<AudioClip>("SoundEffects/plinkoBigCatch");
        }
        else if (catchPoints > 0)
        {
            this.catchAudio.clip = Resources.Load<AudioClip>("SoundEffects/plinkoSmallCatch");
        }        

        this.catchAudio.Play();
    }

    private IEnumerator LoadNextLevel()
    {
        this.catchAudio.clip = Resources.Load<AudioClip>("SoundEffects/plinkoLevelSwitch");
        this.catchAudio.Play();

        while (this.catchAudio.isPlaying)
        {
            yield return null;
        }

        ChatManager.instance.LoadNewPlinkoLevel();
    }
}
