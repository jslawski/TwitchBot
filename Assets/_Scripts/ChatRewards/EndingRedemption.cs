using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingRedemption : BlockerRedemption
{
    [SerializeField]
    private AudioSource[] endingAudio;

    [SerializeField]
    private Text endingText;     

    private List<string> endingTypes = new List<string> { "good", "bad", "what" };

    private int audioIndex = 0;

    private struct EndingData
    {
        public string endingType;
        public string endingMessage;
    }

    public override void TriggerReward(string userRedeemed, string redemptionMessage = "")
    {
        EndingData newEndingData = this.GetParsedEndingData(redemptionMessage);
    
        this.endingText.text = newEndingData.endingMessage;
    
        this.blockerObject.SetActive(true);
        Invoke("HideBlocker", this.timeToDisplay);

        this.PlayEndingAudio(newEndingData.endingType);

        this.PauseAffectedVideos();
    }

    private EndingData GetParsedEndingData(string message)
    {
        EndingData parsedData = new EndingData();

        string[] splitStrings = message.Split(":");

        switch (splitStrings[0].ToLower())
        {
            case "good":                
            case "bad":                
            case "what":
                parsedData.endingType = splitStrings[0].ToLower();
                break;
            default:
                parsedData.endingType = this.GetRandomEndingType();
                parsedData.endingMessage = message;
                return parsedData;               
        }

        string finalMessage = string.Empty;
        for (int i = 1; i < splitStrings.Length; i++)
        {
            finalMessage += splitStrings[i];
        }

        parsedData.endingMessage = finalMessage.TrimStart(' ');

        return parsedData;
    }

    private string GetRandomEndingType()
    {
        int randomIndex = Random.Range(0, this.endingTypes.Count);
        return this.endingTypes[randomIndex];
    }

    private void PlayEndingAudio(string endingType)
    {
        switch (endingType)
        {
            case "good":
                this.audioIndex = 0;
                break;
            case "bad":
                this.audioIndex = 1;
                break;
            case "what":            
                this.audioIndex = 2;
                break;
            default:
                Debug.LogError("Unknown ending type: " + endingType + ". Unable to play ending audio");
                return;
        }

        this.endingAudio[this.audioIndex].Play();
    }

    protected override void HideBlocker()
    {
        base.HideBlocker();

        this.endingAudio[this.audioIndex].Stop();
    }
}
