using CabbageNetwork;
using UnityEngine;

public class UpdateStreamDatesAsyncRequest : AsyncRequest
{
    public UpdateStreamDatesAsyncRequest(NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "twitchBot/updateStreamDates.php";

        this.form = new WWWForm();

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
