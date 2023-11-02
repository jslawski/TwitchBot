using UnityEngine;
using CabbageNetwork;

public class GetCabbageLeaderboardEntryAsyncRequest : AsyncRequest
{
    public GetCabbageLeaderboardEntryAsyncRequest(string chatterName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/leaderboard/getCabbageLeaderboardEntry.php";

        this.form = new WWWForm();
        this.form.AddField("username", chatterName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
