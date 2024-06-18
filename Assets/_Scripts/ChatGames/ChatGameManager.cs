using UnityEngine;

public class ChatGameManager : MonoBehaviour
{
    private BBallGame bballGame;
    private PlinkoGame plinkoGame;
    private FishingGame fishingGame;

    [SerializeField]
    private LeaderboardManager leaderboardManager;

    private void Awake()
    {
        this.bballGame = GetComponentInChildren<BBallGame>();
        this.plinkoGame = GetComponentInChildren<PlinkoGame>();
        this.fishingGame = GetComponentInChildren<FishingGame>();
    }

    public bool IsChatGameActive()
    {
        return (this.bballGame.gameActive == true || this.plinkoGame.gameActive == true || this.fishingGame.gameActive == true);
    }

    public bool IsBBallActive()
    {
        return this.bballGame.gameActive;
    }

    public bool IsPlinkoActive()
    {
        return this.plinkoGame.gameActive;
    }

    public bool IsFishingActive()
    {
        return this.fishingGame.gameActive;
    }

    public void ProcessGameActivationCommand(string commandText)
    {
        if (commandText == "bball")
        {
            this.bballGame.ToggleGame(this.leaderboardManager);
        }
        else if (commandText == "plinko")
        {
            this.plinkoGame.ToggleGame(this.leaderboardManager);
        }
        else if (commandText == "test")
        {
            this.fishingGame.ToggleGame(this.leaderboardManager);
        }
    }

    public void ProcessGameCommand(string username, string commandText, string arguments = "")
    {
        if (this.bballGame.gameActive == true)
        {
            this.bballGame.ProcessCommand(username, commandText, arguments);
        }

        if (this.plinkoGame.gameActive == true)
        {
            this.plinkoGame.ProcessCommand(username, commandText, arguments);
        }

        if (this.fishingGame.gameActive == true)
        {
            this.fishingGame.ProcessCommand(username, commandText, arguments);
        }
    }
}
