using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteMessageBox : MonoBehaviour
{
    public const string TwitchEmoteUrlStub = "https://static-cdn.jtvnw.net/emoticons";
    public const string BTTVEmoteUrlStub = "https://cdn.betterttv.net/emote";
    public const string FrankerFacezEmoteUrlStub = "https://cdn.frankerfacez.com/emoticon";
    private List<string> bttvEmoteNames;

    private Dictionary<string, string> thirdPartyEmoteDict;

    public GameObject emoteBoxObject;
    public GameObject messageBoxObject;
    public GameObject chatLineObject;

    private GameObject currentChatLine;

    private int messageUptime = 15;

    public RectTransform currentChatLineRect;
    public RectTransform parentRect;
    public VerticalLayoutGroup parentVerticalLayoutGroup;

    private void Awake()
    {
        this.InitializeThirdPartyEmotes(); 

        this.parentRect = this.transform.parent.transform.parent.GetComponent<RectTransform>();
        this.parentVerticalLayoutGroup = this.transform.parent.GetComponent<VerticalLayoutGroup>();
    }

    private void InitializeThirdPartyEmotes()
    {
        this.bttvEmoteNames = new List<string>{"catJAM", "AquaTriggered", "LOADING", "dekuHYPE", "nutButton", "CouldYouNot", "OOOO", "Clap", "coffinPls", "pepeD", "ddHuh"};

        this.thirdPartyEmoteDict = new Dictionary<string, string>
        {
            { "EZ", "https://cdn.betterttv.net/emote/5590b223b344e2c42a9e28e3/1x" },
            { "ThisIsFine", "https://cdn.betterttv.net/emote/5823dfea4ccad28a2102dd5b/1x" },
            { "Tuturu", "https://cdn.betterttv.net/emote/55371944236a1aa17a9970a8/1x" },
            { "ZOINKS", "https://cdn.betterttv.net/emote/5b1741ee83deca65adc4a3c6/1x" },
            { "FeelsBadMan", "https://cdn.frankerfacez.com/emoticon/33355/1" },
            { "KEKW", "https://cdn.frankerfacez.com/emoticon/381875/1" },
            { "monkaHmm", "https://cdn.frankerfacez.com/emoticon/240746/1" },
            { "monkaS", "https://cdn.frankerfacez.com/emoticon/130762/1" },
            { "monkaSpeed", "https://cdn.frankerfacez.com/emoticon/260557/1" },
            { "monkaTOS", "https://cdn.frankerfacez.com/emoticon/237857/1" },
            { "OMEGALUL", "https://cdn.frankerfacez.com/emoticon/128054/1" },
            { "PepeHands", "https://cdn.frankerfacez.com/emoticon/231552/1" },
            { "POGGERS", "https://cdn.frankerfacez.com/emoticon/214129/1" },
            { "Stonks", "https://cdn.frankerfacez.com/emoticon/428011/1" },
            { "weSmart", "https://cdn.frankerfacez.com/emoticon/165742/1" },
            { "YEP", "https://cdn.frankerfacez.com/emoticon/418189/1" },
        };
    }

    public void DisplayMessage(string emoteMessage)
    {
        StartCoroutine(this.ParseEmoteMessage(emoteMessage));
        StartCoroutine(this.DestroyMessageAfterDelay());
    }

    private bool IsEmote(string word)
    {
        return (word.Contains(EmoteMessageBox.TwitchEmoteUrlStub) || 
            word.Contains(EmoteMessageBox.BTTVEmoteUrlStub) || 
            word.Contains(EmoteMessageBox.FrankerFacezEmoteUrlStub));
    }

    public IEnumerator ParseEmoteMessage(string emoteMessage)
    {
        //Debug.LogError(emoteMessage);
        string[] words = emoteMessage.Split(' ');
        bool previousWordWasText = false;
        MessageBox currentMessageBox = messageBoxObject.GetComponent<MessageBox>();

        this.CreateNewChatLine();

        foreach (string currentWord in words)
        {
            //Is it an emote?
            if (this.IsEmote(currentWord) || this.IsThirdPartyEmote(currentWord))
            {
                GameObject newEmote = Instantiate(emoteBoxObject, this.currentChatLine.transform) as GameObject;
                EmoteBox newEmoteBox = newEmote.GetComponent<EmoteBox>();

                if (this.thirdPartyEmoteDict.ContainsKey(currentWord))
                {
                    newEmoteBox.LoadEmote(this.thirdPartyEmoteDict[currentWord]);
                }
                else
                {
                    newEmoteBox.LoadEmote(currentWord);
                }
                
                //Wait for emote to load
                int maxAttempts = 100;
                for (int i = 0; i < maxAttempts; i++)
                {
                    if (newEmoteBox.emoteImage.sprite != null || newEmoteBox.gifEmote != null)
                    {
                        break;
                    }
                    yield return null;
                }
                
                yield return new WaitForSeconds(0.01f);

                //Create a linebreak if the width of the parent rect gets exceeded
                if (this.currentChatLineRect.rect.width >= (this.parentRect.rect.width - this.parentVerticalLayoutGroup.padding.left - this.parentVerticalLayoutGroup.padding.right))
                {
                    this.CreateNewChatLine();
                    newEmote.transform.SetParent(this.currentChatLine.transform);
                }

                previousWordWasText = false;
            }
            else
            {
                //Create a new textbox if it's the first one, or the previous word was an emote
                if (currentMessageBox.messageText.text == string.Empty || previousWordWasText == false)
                {
                    currentMessageBox = this.CreateNewMessageBox(currentWord);
                    yield return new WaitForFixedUpdate();
                    //Debug.LogError("Current Chatline Width: " + this.currentChatLineRect.rect.size.x);
                }
                //Append to an existing MessageBox
                else
                {
                    currentMessageBox.AppendText(currentWord);

                    //Wait a bit for Layout sizes to update
                    yield return new WaitForSeconds(0.01f);
                    /*
                    Debug.LogError("Current Chatline Width: " + this.currentChatLineRect.rect.size.x);
                    Debug.LogError("Parent Rect Width: " + this.parentRect.rect.width);
                    Debug.LogError("Left Padding: " + this.parentVerticalLayoutGroup.padding.left);
                    Debug.LogError("Right Padding: " + this.parentVerticalLayoutGroup.padding.right);
                    */
                    //Create a linebreak if the width of the parent rect gets exceeded
                    if (this.currentChatLineRect.rect.width >= (this.parentRect.rect.width - this.parentVerticalLayoutGroup.padding.left - this.parentVerticalLayoutGroup.padding.right))
                    {
                        currentMessageBox.RemoveLastWord(currentWord.Length);
                        this.CreateNewChatLine();
                        currentMessageBox = this.CreateNewMessageBox(currentWord);
                    }
                }

                previousWordWasText = true;
            }
        }
    }

    private MessageBox CreateNewMessageBox(string currentWord)
    {
        GameObject newMessage = Instantiate(messageBoxObject, this.currentChatLine.transform) as GameObject;
        MessageBox newMessageBox = newMessage.GetComponent<MessageBox>();
        newMessageBox.SetText(currentWord);
        return newMessageBox;
    }

    private void CreateNewChatLine()
    {
        this.currentChatLine = Instantiate(this.chatLineObject, this.gameObject.transform) as GameObject;
        this.currentChatLineRect = this.currentChatLine.GetComponent<RectTransform>();
    }

    private IEnumerator DestroyMessageAfterDelay()
    {
        yield return new WaitForSeconds(this.messageUptime);
        Destroy(this.gameObject);
    }

    private bool IsThirdPartyEmote(string currentWord)
    {
        return (this.bttvEmoteNames.Contains(currentWord) || this.thirdPartyEmoteDict.ContainsKey(currentWord));
    }

    public void DestroyEarly()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
