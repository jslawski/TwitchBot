using UnityEngine;
using CabbageNetwork;

public class GetCabbageLeaderboardAsyncRequest : AsyncRequest
{
    public GetCabbageLeaderboardAsyncRequest(NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/leaderboard/getCabbageLeaderboard.php";

        this.form = new WWWForm();

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
