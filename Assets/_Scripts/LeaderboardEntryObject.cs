using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterCustomizer;
using UnityEngine.Networking;

public class LeaderboardEntryObject : MonoBehaviour
{
    public TextMeshProUGUI username;
    [SerializeField]
    private CabbageCharacter character;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        if (this.scoreText.text == "0")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void UpdateEntry(string username, float score)
    {
        if (username == string.Empty || score == 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);

        this.character.UpdateCharacter(username);
        
        this.username.text = username;
        this.scoreText.text = Mathf.RoundToInt(score).ToString();
    }    
}
