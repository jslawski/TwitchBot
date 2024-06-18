using TMPro;
using UnityEngine;
using UnityEngine.Video;

//Clear the rendertexture before you play the video to prevent black screen nonsense

public class AchievementRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject achievementObject;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI achievementText;
    [SerializeField]
    private RenderTexture achievementRenderTexture;
    [SerializeField]
    private VideoPlayer videoPlayer;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        this.achievementObject.SetActive(true);
        this.DisplayAchievement(redemptionMessage);
    }

    public void DisplayAchievement(string message)
    {
        int randomScore = this.GetRandomScore();

        this.achievementRenderTexture.Release();

        this.videoPlayer.frame = 0;
        this.videoPlayer.Play();

        this.scoreText.text = randomScore.ToString() + "G - ";
        this.achievementText.text = message;

        Invoke("DisableAchievement", 7f);
    }

    private int GetRandomScore()
    {
        int[] scoreArray = { 69, 420, 100, 50, 150, 250, 5, 10, 300, 500 };

        int randomScoreIndex = Random.Range(0, scoreArray.Length);

        return scoreArray[randomScoreIndex];
    }

    private void DisableAchievement()
    {
        this.achievementRenderTexture.Release();

        this.videoPlayer.frame = 0;
        this.videoPlayer.Stop();

        this.achievementObject.SetActive(false);
    }
}
