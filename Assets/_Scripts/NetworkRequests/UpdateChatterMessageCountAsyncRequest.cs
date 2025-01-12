using CabbageNetwork;
using UnityEngine;

public class UpdateChatterMessageCountAsyncRequest : AsyncRequest
{
    public UpdateChatterMessageCountAsyncRequest(string username, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/updateChatterMessageCount.php";

        this.form = new WWWForm();
        this.form.AddField("username", username);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
