using CabbageNetwork;
using UnityEngine;

public class UpdateChatterPointCountAsyncRequest : AsyncRequest
{
    public UpdateChatterPointCountAsyncRequest(string username, string value, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/updateChatterPointCount.php";

        this.form = new WWWForm();
        this.form.AddField("username", username);
        this.form.AddField("increment", value);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
