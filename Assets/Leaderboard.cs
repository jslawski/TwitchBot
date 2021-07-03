using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullLeaderboardEntry
{
    public CabbageChatter chatter;
    public int score;

    public FullLeaderboardEntry()
    {
        this.chatter = null;
        this.score = int.MinValue;
    }

    public FullLeaderboardEntry(CabbageChatter chatter, int score)
    {
        this.chatter = chatter;
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
            if (this.fullLeaderboard[i].chatter == chatter)
            {
                tempEntry = new FullLeaderboardEntry(this.fullLeaderboard[i].chatter, this.fullLeaderboard[i].score);
                this.fullLeaderboard.RemoveAt(i);
            }
        }

        if (tempEntry.chatter == null)
        {
            this.InsertNewEntry(chatter);
        }
        else
        {
            this.UpdateCreatedEntry(tempEntry);
        }
    }

    private void InsertNewEntry(CabbageChatter chatter)
    {
        FullLeaderboardEntry newEntry = new FullLeaderboardEntry(chatter, 1);

        this.UpdateCreatedEntry(newEntry);
    }

    private void UpdateCreatedEntry(FullLeaderboardEntry entry)
    {
        entry.score = entry.chatter.shootScore;

        bool inserted = false;

        for (int i = 0; i < this.fullLeaderboard.Count; i++)
        {
            Debug.LogError("New Entry Score: " + entry.score + " Existing Score: " + this.fullLeaderboard[i].score);

            if (entry.score >= this.fullLeaderboard[i].score)
            {
                Debug.LogError("Inserting ahead of existing entry");
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
            Debug.LogError("Inserting at beginning of list");
            this.fullLeaderboard.Insert(0, entry);
        }

        if (this.fullLeaderboard.Count == 0)
        {
            this.fullLeaderboard.Add(entry);
        }

        string currentLeaderboard = string.Empty;
        for (int i = 0; i < this.fullLeaderboard.Count; i++)
        {
            currentLeaderboard += this.fullLeaderboard[i].chatter.chatterName + ", ";
        }

        Debug.LogError("Leaderboard: " + currentLeaderboard);

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

            this.topLeaders[i].UpdateEntry(this.fullLeaderboard[leaderIndex].chatter);
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

    /*public void UpdateLeaderboard(CabbageChatter chatter)
    {
        for (int i = 0; i < this.leaderboard.Length; i++)
        {
            if (chatter.shootScore > this.leaderboard[i].score)
            {
                //this.tempLeaderboardEntry = new LeaderboardEntry(this.leaderboard[i].headPiece.sprite, this.leaderboard[i].eyeBrows.sprite, this.leaderboard[i].eyes.sprite, this.leaderboard[i].nose.sprite, this.leaderboard[i].mouth.sprite, this.leaderboard[i].username.text,this.leaderboard[i].score);
                this.ReplacePlacementsBelowIndex(i);
                this.leaderboard[i].UpdateEntry(chatter);
                //this.RemoveRepeatEntries();

                break;
            }
        }
    }

    /*public void UpdateLeaderboard(CabbageChatter chatter)
    {
        if (chatter.shootScore < this.leaderboard[3].score)
        {
            return;
        }

        int maxValue = 1000;
        for (int i = 0; i < this.leaderboard.Length; i++)
        {
            int leaderboardIndex = this.GetMaxScoreBelowValue(maxValue);
            if (leaderboardIndex == -1)
            {
                return;
            }

            CabbageChatter leaderboardChatter = ChatManager.instance.currentActiveChatters[leaderboardIndex];
            maxValue = leaderboardChatter.shootScore;
            this.ReplacePlacement(i, leaderboardChatter);
        }
    }

    private int GetMaxScoreBelowValue(int maxValue)
    {
        int currentMax = 0;
        int maxIndex = -1;
        for (int i = 0; i < ChatManager.instance.currentActiveChatters.Count; i++)
        {
            int checkScore = ChatManager.instance.currentActiveChatters[i].shootScore;
            string checkName = ChatManager.instance.currentActiveChatters[i].username.text;
            if (checkScore > currentMax && checkScore <= maxValue && IsRepeatEntry(checkName) == false)
            {
                currentMax = checkScore;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private bool IsRepeatEntry(string username)
    {
        for (int i = 0; i < this.leaderboard.Length; i++)
        {
            if (username == this.leaderboard[i].username.text)
            {
                return true;
            }
        }

        return false;
    }

    private void ReplacePlacement(int dictIndex, CabbageChatter chatter)
    {
        this.leaderboard[dictIndex].UpdateEntry(chatter);
    }

    private void ReplacePlacementsBelowIndex(int index)
    {
        //Nothing needs to be pushed down if we're just updating an existing leaderboard entry
        if (this.tempLeaderboardEntry.username.text == this.leaderboard[index].username.text)
        {
            return;
        }

        if (index == 3)
        {
            return;
        }

        this.leaderboard[index + 1].ReplaceWithEntry(this.tempLeaderboardEntry);
        

        LeaderboardEntry tempEntry = this.leaderboard[index];
        LeaderboardEntry nextEntry;
        for (int i = index; i < this.leaderboard.Length - 1; i++)
        {
            nextEntry = this.leaderboard[i + 1];

            this.leaderboard[i + 1].ReplaceWithEntry(tempEntry);

            tempEntry = nextEntry;
        }
    }

    private void RemoveRepeatEntries()
    {
        for (int i = 1; i < this.leaderboard.Length; i++)
        {
            if (this.leaderboard[i - 1].username.text == this.leaderboard[i].username.text)
            {
                this.leaderboard[i].gameObject.SetActive(false);
            }
        }
    }*/
}
