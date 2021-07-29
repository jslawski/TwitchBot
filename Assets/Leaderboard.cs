using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullLeaderboardEntry
{
    public Sprite baseCabbage;
    public Sprite headPiece;
    public Sprite eyeBrows;
    public Sprite eyes;
    public Sprite nose;
    public Sprite mouth;

    public string username;
    public int score;

    public FullLeaderboardEntry()
    {
        this.baseCabbage = null;
        this.headPiece = null;
        this.eyeBrows = null;
        this.eyes = null;
        this.nose = null;
        this.mouth = null;
        this.username = string.Empty;
        this.score = int.MinValue;
    }

    public FullLeaderboardEntry(CabbageChatter chatter, int score)
    {
        this.baseCabbage = chatter.baseCabbage.sprite;
        this.headPiece = chatter.headPiece.sprite;
        this.eyeBrows = chatter.eyeBrows.sprite;
        this.eyes = chatter.eyes.sprite;
        this.nose = chatter.nose.sprite;
        this.mouth = chatter.mouth.sprite;
        this.username = chatter.username.text;
        this.score = score;
    }
}

public class Leaderboard : MonoBehaviour
{ 
    public static Leaderboard instance;

    public LeaderboardEntry[] topLeaders;

    private LeaderboardEntry tempLeaderboardEntry;

    private List<FullLeaderboardEntry> fullLeaderboard;

    private void Awake()
    {
        instance = this;
        this.fullLeaderboard = new List<FullLeaderboardEntry>();
        this.UpdateLeaderboardVisuals();
    }

    public void UpdateLeaderboard(CabbageChatter chatter)
    {
        FullLeaderboardEntry tempEntry = new FullLeaderboardEntry();
        for (int i = 0; i < this.fullLeaderboard.Count; i++)
        {
            if (this.fullLeaderboard[i].username == chatter.username.text)
            {
                tempEntry = new FullLeaderboardEntry(chatter, this.fullLeaderboard[i].score);
                this.fullLeaderboard.RemoveAt(i);
            }
        }

        if (tempEntry.username == string.Empty)
        {
            this.InsertNewEntry(chatter);
        }
        else
        {
            tempEntry.score = chatter.shootScore;
            this.UpdateCreatedEntry(tempEntry);
        }

        this.UpdateCrown(chatter.username.text);
    }

    private void UpdateCrown(string scorerName)
    {
        if (ChatManager.instance.lastLeader != null)
        {
            ChatManager.instance.lastLeader.DeactivateCrown();
        }

        ChatManager.instance.chatterDict[this.topLeaders[0].username.text].ActivateCrown();
        ChatManager.instance.lastLeader = ChatManager.instance.chatterDict[this.topLeaders[0].username.text];
    }

    private void InsertNewEntry(CabbageChatter chatter)
    {
        FullLeaderboardEntry newEntry = new FullLeaderboardEntry(chatter, 1);

        this.UpdateCreatedEntry(newEntry);
    }

    private void UpdateCreatedEntry(FullLeaderboardEntry entry)
    {
        bool inserted = false;

        for (int i = this.fullLeaderboard.Count - 1; i >= 0; i--)
        {
            if (entry.score >= this.fullLeaderboard[i].score)
            {
                //Insert entry 1 position ahead of the first entry it is >= than
                //If we are at the end of the list, just add a new entry
                if ((i + 1) < this.fullLeaderboard.Count)
                {
                    this.fullLeaderboard.Insert(i + 1, entry);
                }
                else
                {
                    this.fullLeaderboard.Add(entry);
                }
                
                inserted = true;
                break;
            }
        }

        //Insert the entry to the bottom of the leaderboard if its score doesn't surpass any existing entries
        if (inserted == false)
        {
            this.fullLeaderboard.Insert(0, entry);
        }

        if (this.fullLeaderboard.Count == 0)
        {
            this.fullLeaderboard.Add(entry);
        }

        /*string currentLeaderboard = string.Empty;
        for (int i = this.fullLeaderboard.Count - 1; i >= 0; i--)
        {
            currentLeaderboard += this.fullLeaderboard[i].username + ": " + this.fullLeaderboard[i].score + "\n";
        }

        Debug.LogError("Leaderboard: \n" + currentLeaderboard);
        */
        this.UpdateLeaderboardVisuals();
    }

    private void UpdateLeaderboardVisuals()
    {
        int currentLeaderboardIndex = this.topLeaders.Length - 1;
        for (int i = 0; i < this.topLeaders.Length; i++)
        {
            int leaderIndex = (this.fullLeaderboard.Count - 1) - i;

            if (leaderIndex < 0)
            {
                break;
            }

            this.topLeaders[i].UpdateEntry(this.fullLeaderboard[leaderIndex]);
            this.topLeaders[i].gameObject.SetActive(true);
            currentLeaderboardIndex--;
        }

        //Disable any empty leaderboard entries
        for (int i = 0; i < this.topLeaders.Length; i++)
        {
            if (this.topLeaders[i].score == 0)
            {
                this.topLeaders[i].gameObject.SetActive(false);
            }
        }
    }
}
