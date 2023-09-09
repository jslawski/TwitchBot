using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementUnlocked : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI achievementText;    

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

    private void DestroyAchievement()
    {
        Destroy(this.gameObject);
    }
}
