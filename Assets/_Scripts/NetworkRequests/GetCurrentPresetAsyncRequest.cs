using UnityEngine;
using CabbageNetwork;

public class GetCurrentPresetAsyncRequest : AsyncRequest
{
    public GetCurrentPresetAsyncRequest(string chatterName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/getCurrentPreset.php";

        this.form = new WWWForm();
        this.form.AddField("username", chatterName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
