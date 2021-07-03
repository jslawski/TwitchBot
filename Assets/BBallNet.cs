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
        this.scoreText.text = scorer.username.text + " Scored!";
        this.audienceAudio.Play();
        this.airhornAudio.Play();
        this.niceShotAudio.Play();
        scorer.shootScore++;
        ChatManager.instance.chatterScoreHistory[scorer.username.text] = scorer.shootScore;
        Leaderboard.instance.UpdateLeaderboard(scorer);

        //Toggle crown if necessary
        if (Leaderboard.instance.topLeaders[0].username.text == scorer.username.text)
        {
            //Activate crown on new leader
            scorer.ActivateCrown();

            //Deactivate crown on dethroned leader
            if (ChatManager.instance.chatterDict.ContainsKey(Leaderboard.instance.topLeaders[1].username.text))
            {
                ChatManager.instance.chatterDict[Leaderboard.instance.topLeaders[1].username.text].DeactivateCrown();
            }
        }

        StopAllCoroutines();
        StartCoroutine(this.TurnOffScoreTextAfterDelay());
    }

    private IEnumerator TurnOffScoreTextAfterDelay()
    {
        yield return new WaitForSeconds(this.scoreCooldown);
        this.scoreText.text = string.Empty;
    }
}
