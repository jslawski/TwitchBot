using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    public Image baseCabbage;
    public Image headPiece;
    public Image eyeBrows;
    public Image eyes;
    public Image nose;
    public Image mouth;

    public TextMeshProUGUI username;
    public TextMeshProUGUI scoreText;

    public int score = 0;

    public LeaderboardEntry(Sprite headPiece, Sprite eyebrows, Sprite eyes, Sprite nose, Sprite mouth, string username, int score)
    {
        this.headPiece.sprite = headPiece;
        this.eyeBrows.sprite = eyebrows;
        this.eyes.sprite = eyes;
        this.nose.sprite = nose;
        this.mouth.sprite = mouth;
        this.username.text = username;
        this.scoreText.text = score.ToString();
        this.score = score;
    }

    public LeaderboardEntry(FullLeaderboardEntry chatterEntry)
    {
        this.headPiece.sprite = chatterEntry.headPiece;
        this.eyeBrows.sprite = chatterEntry.eyeBrows;
        this.eyes.sprite = chatterEntry.eyes;
        this.nose.sprite = chatterEntry.nose;
        this.mouth.sprite = chatterEntry.mouth;
        this.username.text = chatterEntry.username;
        this.scoreText.text = chatterEntry.score.ToString();
        this.score = chatterEntry.score;
    }

    public void UpdateEntry(FullLeaderboardEntry chatterEntry)
    {
        this.baseCabbage.sprite = chatterEntry.baseCabbage;
        if (this.baseCabbage.sprite.name != "cabbage")
        {
            this.headPiece.gameObject.SetActive(false);
            this.eyeBrows.gameObject.SetActive(false);
            this.eyes.gameObject.SetActive(false);
            this.nose.gameObject.SetActive(false);
            this.mouth.gameObject.SetActive(false);
        }
        else
        {
            this.headPiece.gameObject.SetActive(true);
            this.eyeBrows.gameObject.SetActive(true);
            this.eyes.gameObject.SetActive(true);
            this.nose.gameObject.SetActive(true);
            this.mouth.gameObject.SetActive(true);
        }

        this.headPiece.sprite = chatterEntry.headPiece;
        this.eyeBrows.sprite = chatterEntry.eyeBrows;
        this.eyes.sprite = chatterEntry.eyes;
        this.nose.sprite = chatterEntry.nose;
        this.mouth.sprite = chatterEntry.mouth;

        this.username.text = chatterEntry.username;
        this.scoreText.text = chatterEntry.score.ToString();
        this.score = chatterEntry.score;
    }

    public void ReplaceWithEntry(LeaderboardEntry entry)
    {
        this.headPiece.sprite = entry.headPiece.sprite;
        this.eyeBrows.sprite = entry.eyeBrows.sprite;
        this.eyes.sprite = entry.eyes.sprite;
        this.nose.sprite = entry.nose.sprite;
        this.mouth.sprite = entry.mouth.sprite;

        this.username.text = entry.username.text;
        this.scoreText.text = entry.score.ToString();
        this.score = entry.score;
    }
}
