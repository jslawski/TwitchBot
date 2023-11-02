using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmoteBox : MonoBehaviour
{
    public Image emoteImage;
    public Animator gifEmote;

    private bool IsStaticEmote(string word)
    {
        return (word.Contains(EmoteMessageBox.TwitchEmoteUrlStub) ||
            word.Contains(EmoteMessageBox.BTTVEmoteUrlStub) ||
            word.Contains(EmoteMessageBox.FrankerFacezEmoteUrlStub));
    }

    public void LoadEmote(string emoteUrl)
    {
        if (this.IsStaticEmote(emoteUrl))
        {
            StartCoroutine(GetEmoteFromUrl(emoteUrl));
        }
        else
        {
            this.gifEmote.Play(emoteUrl);
        }
    }

    private IEnumerator GetEmoteFromUrl(string emoteUrl)
    {
        using (UnityWebRequest emoteRequest = UnityWebRequestTexture.GetTexture(emoteUrl))
        {
            yield return emoteRequest.SendWebRequest();
            
            if (emoteRequest.responseCode != 200L)
            {
                Debug.LogError(emoteRequest.error + " Response Code: " + emoteRequest.responseCode);
            }
            else
            {
                Texture2D emoteTexture = DownloadHandlerTexture.GetContent(emoteRequest);

                Rect emoteRect = new Rect(0, 0, emoteTexture.width, emoteTexture.height);
                emoteImage.sprite = Sprite.Create(emoteTexture, emoteRect, new Vector2(0, 0));
            }
        }
    }
}
