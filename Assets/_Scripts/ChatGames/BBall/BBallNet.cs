using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BBallNet : MonoBehaviour
{
    [SerializeField]
    private BoxCollider netCollider;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    private float scoreCooldown = 5f;
    [SerializeField]
    private AudioSource audienceAudio;
    [SerializeField]
    private AudioSource airhornAudio;
    [SerializeField]
    private AudioSource niceShotAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CabbageChatter potentialScorer = other.gameObject.GetComponent<CabbageChatter>();

        if (potentialScorer != null)
        {
            this.InitiateScoreSequence(potentialScorer);
        }
    }

    private void InitiateScoreSequence(CabbageChatter scorer)
    {
        this.audienceAudio.Play();
        this.airhornAudio.Play();
        this.niceShotAudio.Play();
        scorer.shootScore++;

        this.scoreText.text = scorer.chatterName + " Scored!\n" + scorer.shootScore.ToString() + "pts";

        while (scorer.shootScore >= CabbageManager.instance.prestigeThreshold)
        {
            scorer.TriggerPrestige();
        }

        Leaderboard.instance.QueueLeaderboardUpdate(scorer.chatterName, 3.0f);

        StopAllCoroutines();
        StartCoroutine(this.TurnOffScoreTextAfterDelay());
    }

    private IEnumerator TurnOffScoreTextAfterDelay()
    {
        yield return new WaitForSeconds(this.scoreCooldown);
        this.scoreText.text = string.Empty;
    }
}
