using TMPro;
using UnityEngine;

//Clear the rendertexture before you play the video to prevent black screen nonsense

public class AchievementRedemption : RewardRedemption
{
    [SerializeField]
    private GameObject achievementObject;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI achievementText;

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        Vector2 achievementViewportPosition = new Vector2(0.5f, 0.1f);
        Vector2 achievementWorldPosition = Camera.main.ViewportToWorldPoint(achievementViewportPosition);

        Vector3 achievementWorldPosition3D = new Vector3(achievementWorldPosition.x, achievementWorldPosition.y, -1.0f);

        GameObject achievementInstance = Instantiate(this.achievementObject, achievementWorldPosition3D, new Quaternion()) as GameObject;
        this.achievementObject.SetActive(true);
        this.DisplayAchievement(redemptionMessage);
    }

    public void DisplayAchievement(string message)
    {
        int randomScore = this.GetRandomScore();

        this.scoreText.text = randomScore.ToString() + "G - ";
        this.achievementText.text = message;

        Invoke("DestroyAchievement", 7f);
    }

    private int GetRandomScore()
    {
        int[] scoreArray = { 69, 420, 100, 50, 150, 250, 5, 10, 300, 500 };

        int randomScoreIndex = Random.Range(0, scoreArray.Length);

        return scoreArray[randomScoreIndex];
    }

    private void DisableAchievement()
    {
        this.achievementObject.SetActive(false);
    }
}
