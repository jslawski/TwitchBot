using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCustomizer;
using UnityEngine.Networking;

public class LeaderboardUpdate
{
    public string username;
    public float value;

    public LeaderboardUpdate(string newName, float newValue)
    {
        this.username = newName;
        this.value = newValue;
    }
}

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;

    [SerializeField]
    private GameObject parentChatObject;

    [HideInInspector]
    public LeaderboardEntryObject[] entries;

    private Queue<LeaderboardUpdate> queuedUpdates;

    private bool readyToProcessUpdate = true;

    public LeaderboardData currentLeaderboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.entries = GetComponentsInChildren<LeaderboardEntryObject>(true);
        this.queuedUpdates = new Queue<LeaderboardUpdate>();        
    }

    public void RefreshLeaderboard()
    {
        this.RequestLeaderboard();
    }

    #region Get Leaderboard
    private void RequestLeaderboard()
    {
        GetCabbageLeaderboardAsyncRequest request = new GetCabbageLeaderboardAsyncRequest(this.RequestLeaderboardSuccess, this.RequestLeaderboardFailure);
        request.Send();
    }

    private void RequestLeaderboardSuccess(string data)
    {
        this.currentLeaderboard = JsonUtility.FromJson<LeaderboardData>(data);
        this.UpdateLeaderboardVisuals();
    }

    private void RequestLeaderboardFailure()
    {
        Debug.LogError("Error: Unable to get leaderboard");
    }
    #endregion

    #region UpdateLeaderboard
    private void UpdateLeaderboard(LeaderboardUpdate updateValues)
    {
        UpdateCabbageLeaderboardAsyncRequest request = new UpdateCabbageLeaderboardAsyncRequest(updateValues.username, updateValues.value.ToString(), this.UpdateLeaderboardSuccess, this.UpdateLeaderboardFailure);
        request.Send();
    }

    private void UpdateLeaderboardSuccess(string data)
    {
        this.currentLeaderboard = JsonUtility.FromJson<LeaderboardData>(data);
        this.UpdateLeaderboardVisuals();
        this.readyToProcessUpdate = true;
    }

    private void UpdateLeaderboardFailure()
    {
        Debug.LogError("Error: Unable to update leaderboard entry");
    }
    #endregion


    public LeaderboardEntryData GetTopPlayer()
    {
        if (this.currentLeaderboard.entries.Count > 0)
        {
            return this.currentLeaderboard.entries[0];
        }
        else
        {
            return null;
        }
    }

    public void QueueLeaderboardUpdate(string username, float value)
    {
        this.queuedUpdates.Enqueue(new LeaderboardUpdate(username, value));        
    }

    private void FixedUpdate()
    {
        if (this.queuedUpdates.Count > 0 && this.readyToProcessUpdate == true)
        {
            this.ProcessUpdate(this.queuedUpdates.Dequeue());
        }
    }

    private void ProcessUpdate(LeaderboardUpdate updateValues)
    {
        this.readyToProcessUpdate = false;

        this.UpdateLeaderboard(updateValues);
    }

    private void UpdateLeaderboardVisuals()
    {
        for (int i = 0; i < this.currentLeaderboard.entries.Count && i < this.entries.Length; i++)
        {
            this.entries[i].UpdateEntry(this.currentLeaderboard.entries[i].username, this.currentLeaderboard.entries[i].value);
        }

        this.UpdateCrownHolder();
    }

    //This function is responsible for changing the crown holder in situations where
    //Cabbage Chatters remain on screen after scoring points (unlike plinko)    
    private void UpdateCrownHolder()
    {
        if (this.currentLeaderboard == null)
        {
            return;
        }

        CabbageChatter[] allActiveChatters = this.parentChatObject.GetComponentsInChildren<CabbageChatter>();
        
        for (int i = 0; i < allActiveChatters.Length; i++)
        {
            if (allActiveChatters[i].chatterName == this.currentLeaderboard.entries[0].username)
            {
                allActiveChatters[i].ActivateCrown();
            }
            else
            {
                allActiveChatters[i].DeactivateCrown();
            }
        }
    }
}
