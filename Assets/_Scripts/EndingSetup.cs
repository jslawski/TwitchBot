using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSetup : MonoBehaviour
{
    private AudioClip[] oceanManClips;
    public AudioSource oceanManSource;

    const string LinkPrefixStub = "Today's Ocean Man is: ";
    const string LinkSuffixStub = "!  Check it out at: ";
    const string YoutubeLinkStub = "https://www.youtube.com/watch?v=";

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            this.oceanManClips = Resources.LoadAll<AudioClip>("OceanMan");
        }
    }

    private void PickAndPlayRandomSong()
    {
        this.oceanManSource.clip = this.oceanManClips[Random.Range(0, this.oceanManClips.Length)];
        oceanManSource.Play();
    }

    private void PostSongTitleAndLink()
    {
        string videoTitle = this.oceanManSource.clip.name.Substring(0,this.oceanManSource.clip.name.Length - 12);
        string videoLinkStub = this.oceanManSource.clip.name.Substring(this.oceanManSource.clip.name.Length - 11);

        string fullMessage = LinkPrefixStub + "\"" + videoTitle + "\"" + LinkSuffixStub + YoutubeLinkStub + videoLinkStub;

        ChatManager.instance.chatClient.SendMessage(SecretKeys.ChannelName, fullMessage);
    }

    public void BeginEndSequence()
    {
        if (!Application.isEditor)
        {
            this.PickAndPlayRandomSong();
        }

        this.PostSongTitleAndLink();
    }
}
