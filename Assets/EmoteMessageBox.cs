using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteMessageBox : MonoBehaviour
{
    const string EmoteUrlStub = "https://static-cdn.jtvnw.net/emoticons";
    private Dictionary<string, string> bttvEmoteUrls;
        
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
        this.InitializeBTTVEmotesDict(); 

        this.parentRect = this.transform.parent.transform.parent.GetComponent<RectTransform>();
        this.parentVerticalLayoutGroup = this.transform.parent.GetComponent<VerticalLayoutGroup>();
    }

    private void InitializeBTTVEmotesDict()
    {
        this.bttvEmoteUrls = new Dictionary<string, string>()
        {
            { "catJAM", "https://cdn.betterttv.net/emote/5f1b0186cf6d2144653d2970/1x"},
            { "weSmart", "https://cdn.betterttv.net/emote/589771dc10c0975495c578d1/1x"},
            { "ddHuh", "https://cdn.betterttv.net/emote/58b20d74d07b273e0dcfd57c/1x"},
            { "dekuHYPE", "https://cdn.betterttv.net/emote/594c13b436b6a43b492ce4bd/1x"},
            { "nutButton", "https://cdn.betterttv.net/emote/5e0c5beb89079f7ba7c45b4c/1x"}
        };
    }

    public void DisplayMessage(string emoteMessage)
    {
        StartCoroutine(this.ParseEmoteMessage(emoteMessage));
        StartCoroutine(this.DestroyMessageAfterDelay());
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
            if (currentWord.Contains(EmoteUrlStub) /*|| this.IsBTTVEmote(currentWord)*/)
            {
                GameObject newEmote = Instantiate(emoteBoxObject, this.currentChatLine.transform) as GameObject;
                EmoteBox newEmoteBox = newEmote.GetComponent<EmoteBox>();
                newEmoteBox.LoadEmote(currentWord);

                //Wait for emote to load
                int maxAttempts = 100;
                for (int i = 0; i < maxAttempts; i++)
                {
                    if (newEmoteBox.emoteImage.sprite != null)
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

    private bool IsBTTVEmote(string currentWord)
    {
        return this.bttvEmoteUrls.ContainsKey(currentWord);
    }

    public void DestroyEarly()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
