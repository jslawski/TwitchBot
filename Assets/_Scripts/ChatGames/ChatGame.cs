using System.Collections;
using UnityEngine;

public class ChatGame : MonoBehaviour
{
    public float secondsBetweenAIAction = 45.0f;

    public bool gameActive = false;

    private LeaderboardManager leaderboardManager;

    public void ToggleGame(LeaderboardManager leaderboardManager)
    {
        this.leaderboardManager = leaderboardManager;

        if (this.gameActive == false)
        {
            this.Setup();
            this.leaderboardManager.EnableLeaderboard();
            this.gameActive = true;
        }
        else
        {
            this.Cleanup();
            this.leaderboardManager.DisableLeaderboard();
            this.gameActive = false;
        }
    }

    public virtual void Setup() { }

    public virtual void ProcessCommand(string username, string commandText, string argumentsAsString = "") { }

    public virtual IEnumerator AICoroutine() { yield return null; }

    public virtual void Cleanup() { }
}
