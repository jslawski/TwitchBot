using UnityEngine;
using CabbageNetwork;

public class UpdateCabbageLeaderboardAsyncRequest : AsyncRequest
{
    public UpdateCabbageLeaderboardAsyncRequest(string username, string value, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/leaderboard/updateCabbageLeaderboard.php";

        this.form = new WWWForm();
        this.form.AddField("username", username);
        this.form.AddField("increment", value);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
