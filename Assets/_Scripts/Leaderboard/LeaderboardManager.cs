using System.Collections.Generic;
using UnityEngine;

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

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    [HideInInspector]
    public LeaderboardEntryObject[] leaderboardEntryObjects;

    private Queue<LeaderboardUpdate> queuedUpdates;

    private bool readyToProcessUpdate = true;

    public LeaderboardData currentLeaderboardData;

    [SerializeField]
    private GameObject leaderboardObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.leaderboardEntryObjects = GetComponentsInChildren<LeaderboardEntryObject>(true);
        this.queuedUpdates = new Queue<LeaderboardUpdate>();

        this.RefreshLeaderboard();
    }

    public void EnableLeaderboard()
    {
        this.leaderboardObject.SetActive(true);
        this.RefreshLeaderboard();
    }

    public void DisableLeaderboard()
    {
        this.leaderboardObject.SetActive(false);
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
        //Empty leaderboard, return
        if (data == "[]")
        {
            return;
        }

        this.currentLeaderboardData = JsonUtility.FromJson<LeaderboardData>(data);

        if (this.leaderboardObject.activeSelf == true)
        {
            this.UpdateLeaderboardVisuals();
        }
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
        this.currentLeaderboardData = JsonUtility.FromJson<LeaderboardData>(data);
        this.UpdateLeaderboardVisuals();
        this.readyToProcessUpdate = true;
    }

    private void UpdateLeaderboardFailure()
    {
        Debug.LogError("Error: Unable to update leaderboard entry");
        this.readyToProcessUpdate = true;
    }
    #endregion

    public LeaderboardEntryData GetTopPlayer()
    {
        if (this.currentLeaderboardData.entries.Count > 0)
        {
            return this.currentLeaderboardData.entries[0];
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
        this.ClearLeaderboardVisuals();

        for (int i = 0; i < this.currentLeaderboardData.entries.Count && i < this.leaderboardEntryObjects.Length; i++)
        {
            this.leaderboardEntryObjects[i].UpdateEntry(this.currentLeaderboardData.entries[i].username, this.currentLeaderboardData.entries[i].value);
        }

        if (this.currentLeaderboardData.entries.Count > 0)
        {
            CrownManager.UpdateCrownHolder(this.GetTopPlayer().username);
        }
    }

    private void ClearLeaderboardVisuals()
    {
        for (int i = 0; i < this.leaderboardEntryObjects.Length; i++)
        {
            this.leaderboardEntryObjects[i].UpdateEntry(string.Empty, 0);
        }
    }

    public bool IsTopPlayer(string username)
    {
        return (this.GetTopPlayer().username == username);
    }
}
