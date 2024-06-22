using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Models;
using System;
using TwitchLib.Communication.Clients;
using System.Collections;
using CharacterCustomizer;

public class CabbageManager : MonoBehaviour
{
    public static CabbageManager instance;

    public Client chatClient;
    private ConnectionCredentials botCreds = new ConnectionCredentials(SecretKeys.BotName, SecretKeys.AccessToken);

    public GameObject parentChat;
    public GameObject cabbageChatterPrefab;
    public BoxCollider spawnBoundaries;

    public Dictionary<string, CabbageChatter> chatterDict;

    public List<CabbageChatter> currentActiveChatters;

    public Queue<CabbageChatter> chatterQueue;
    private bool readyForNextChatter = true;

    [SerializeField]
    private GameObject leaderboardCanvas;
    
    private const string ClipStub = "https://clips.twitch.tv/";
    public static string recentClip = string.Empty;

    public int prestigeThreshold = 9999999;

    private int currentSortOrder = 0;
    
    [SerializeField]
    private CommandManager commandManager;
    [SerializeField]
    private ChatGameManager chatGameManager;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
        }

        CharacterCache.Setup();

        ClientOptions clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new WebSocketClient(clientOptions);

        this.chatClient = new Client();
        this.chatClient.Initialize(this.botCreds, SecretKeys.ChannelName);
        this.chatClient.OnMessageReceived += this.ProcessMessage;
        this.chatClient.WillReplaceEmotes = true;
        this.chatClient.OnUserLeft += this.RemoveCabbageChatterOnLeave;
        this.chatClient.OnChatCommandReceived += this.ProcessCommand;
        this.chatClient.OnConnected += this.Client_OnConnected;
        this.chatClient.AutoReListenOnException = true;
        this.chatClient.Disconnect();
        this.chatClient.Connect();

        this.chatterDict = new Dictionary<string, CabbageChatter>();
        this.currentActiveChatters = new List<CabbageChatter>();
        this.chatterQueue = new Queue<CabbageChatter>();
        
        StartCoroutine(this.RejoinHeartbeat());

        AttributeSpriteDicts.Setup();
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        this.chatClient.JoinChannel(SecretKeys.ChannelName);        
        this.SendBotMessage("Greetings, professor!  Nothing to report!");
    }

    private void ProcessCommand(object sender, OnChatCommandReceivedArgs e)
    {
        StartCoroutine(this.ProcessCommandDelay(e.Command.ChatMessage.Username.ToLower(), e.Command.CommandText, e.Command.ArgumentsAsString));
    }

    private IEnumerator ProcessCommandDelay(string username, string commandText, string arguments)
    {
        if (username == "ruddgasm" || username == "ruddpuddle" || username == "jerry_jerah_jeremeson" || username == "coleslawski")
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
        }

        this.commandManager.ProcessCommand(username, commandText, arguments);
    }

    private void ProcessMessage(object sender, OnMessageReceivedArgs e)
    {        
        //Supress SoundAlerts bot and commands
        if (e.ChatMessage.Username == "soundalerts" || e.ChatMessage.Message.StartsWith("!"))
        {
            return;
        }
        
        if (e.ChatMessage.Message.Contains(CabbageManager.ClipStub))
        {
            this.GrabClipLink(e.ChatMessage.Message);
        }

        if (this.chatterDict.ContainsKey(e.ChatMessage.Username.ToLower()))
        {
            this.chatterDict[e.ChatMessage.Username.ToLower()].DisplayChatMessage(e.ChatMessage.Username, this.GetProperMessage(e.ChatMessage), this.chatGameManager.IsPlinkoActive());
            this.chatterQueue.Enqueue(this.chatterDict[e.ChatMessage.Username.ToLower()]);
        }
        else if (this.chatGameManager.IsPlinkoActive() == false) //Create a new cabbage chatter
        {
            this.SpawnNewChatter(e.ChatMessage.Username.ToLower(), e.ChatMessage);
        }
    }

    private void ProcessBotMessage(string message)
    {
        if (this.chatterDict.ContainsKey(SecretKeys.BotName))
        {
            this.chatterDict[SecretKeys.BotName].DisplayChatMessage(SecretKeys.BotName, message, this.chatGameManager.IsPlinkoActive());
            this.chatterQueue.Enqueue(this.chatterDict[SecretKeys.BotName]);
        }
        else if (this.chatGameManager.IsPlinkoActive() == false) //Create a new cabbage chatter
        {
            this.SpawnNewBot(message);
        }
    }

    private void SpawnNewBot(string message)
    {
        float randomXPosition = UnityEngine.Random.Range(spawnBoundaries.bounds.min.x, spawnBoundaries.bounds.max.x);
        Vector3 instantiationPosition = new Vector3(randomXPosition, spawnBoundaries.transform.position.y, 0f);
        GameObject newChatter = Instantiate(cabbageChatterPrefab, instantiationPosition, new Quaternion(), this.parentChat.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        this.chatterDict.Add(SecretKeys.BotName, cabbageChatter);
        this.currentActiveChatters.Add(cabbageChatter);
        this.chatterQueue.Enqueue(cabbageChatter);

        cabbageChatter.chatterName = SecretKeys.BotName;
        newChatter.name = SecretKeys.BotName;

        cabbageChatter.UpdateLayer(this.currentSortOrder++);

        if (message != string.Empty)
        {
            cabbageChatter.DisplayChatMessage(SecretKeys.BotName, message);
        }
        else
        {
            cabbageChatter.LoadCharacter();
        }
    }

    public void SpawnNewChatter(string username, ChatMessage newChatterMessage = null)
    {
        float randomXPosition = UnityEngine.Random.Range(spawnBoundaries.bounds.min.x, spawnBoundaries.bounds.max.x);
        Vector3 instantiationPosition = new Vector3(randomXPosition, spawnBoundaries.transform.position.y, 0f);
        GameObject newChatter = Instantiate(cabbageChatterPrefab, instantiationPosition, new Quaternion(), this.parentChat.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        this.chatterDict.Add(username, cabbageChatter);
        this.currentActiveChatters.Add(cabbageChatter);
        this.chatterQueue.Enqueue(cabbageChatter);

        cabbageChatter.chatterName = username;
        newChatter.name = username;

        cabbageChatter.UpdateLayer(this.currentSortOrder++);

        if (this.chatGameManager.IsFishingActive() == true)
        {
            cabbageChatter.fisher.Setup();
            cabbageChatter.SuspendCabbage();
            cabbageChatter.UnsuspendCabbage();
        }

        if (newChatterMessage != null)
        {
            cabbageChatter.DisplayChatMessage(username, this.GetProperMessage(newChatterMessage));
        }
        else
        {
            cabbageChatter.LoadCharacter();
        }
    }

    private string GetProperMessage(ChatMessage chatMessage)
    {
        if (chatMessage.EmoteReplacedMessage != null)
        {
            return chatMessage.EmoteReplacedMessage;
        }
        else
        {
            return chatMessage.Message;
        }
    }

    private void RemoveCabbageChatterOnLeave(object sender, OnUserLeftArgs e)
    {
        this.RemoveCabbage(e.Username);
    }

    public CabbageChatter AddCabbage(string username)
    {
        if (this.chatterDict.ContainsKey(username.ToLower()) == false)
        {
            GameObject newChatter = Instantiate(cabbageChatterPrefab, Vector3.zero, new Quaternion(), this.parentChat.transform) as GameObject;
            CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
            this.chatterDict.Add(username, cabbageChatter);
            this.currentActiveChatters.Add(cabbageChatter);
            return this.chatterDict[username];
        }
        else
        {
            return this.chatterDict[username];
        }
    }

    public void RemoveCabbage(string username)
    {
        if (this.chatterDict.ContainsKey(username.ToLower()))
        {
            Destroy(this.chatterDict[username.ToLower()].gameObject);
            this.chatterDict.Remove(username.ToLower());
        }

        this.currentActiveChatters.Remove(this.currentActiveChatters.Find(x => x.chatterName == username));
    }

    private void PushChatterToFront(CabbageChatter latestChatter)
    {
        this.readyForNextChatter = false;
        //Remove the chatter from it's current position in the list, and put it at the end
        //Check that the chatter still exists, in case it got destroyed while waiting
        if (latestChatter != null)
        {
            this.currentActiveChatters.Remove(latestChatter);
            this.currentActiveChatters.Add(latestChatter);
            latestChatter.UpdateLayer(this.currentSortOrder++);
        }

        this.readyForNextChatter = true;
    }

    private void UpdateChatterLayers()
    {
        for (int i = 0; i < this.currentActiveChatters.Count; i++)
        {
            this.currentActiveChatters[i].UpdateLayer(i);
        }
    }

    private void GrabClipLink(string chatMessage)
    {
        int startIndex = chatMessage.IndexOf(CabbageManager.ClipStub);
        string remainingMessage = chatMessage.Substring(startIndex);
        int endIndex = remainingMessage.IndexOf(" ");
        if (endIndex < 0)
        {
            endIndex = remainingMessage.Length;
        }

        CabbageManager.recentClip = chatMessage.Substring(startIndex, endIndex);
    }

    private void Update()
    {
        if (this.chatterQueue.Count > 0 && this.readyForNextChatter == true)
        {
            this.PushChatterToFront(this.chatterQueue.Dequeue());
        }
    }

    private IEnumerator RejoinHeartbeat()
    {
        while (true)
        {
            this.chatClient.JoinChannel(SecretKeys.ChannelName, true);
            yield return new WaitForSeconds(300);
        }
    }

    private void OnApplicationQuit()
    {
        this.chatClient.Disconnect();
    }

    public bool DoesChatterExist(string username)
    {
        return this.chatterDict.ContainsKey(username);
    }

    public bool TestChatterExists()
    {
        for (int i = 0; i < this.currentActiveChatters.Count; i++)
        {
            if (this.currentActiveChatters[i].chatterName.Contains("testcabbage") == true)
            {
                return true;
            }
        }

        return false;
    }

    public CabbageChatter GetCabbageChatter(string username)
    {
        return this.chatterDict[username];
    }

    public void SendBotMessage(string message)
    {
        if (this.chatClient.JoinedChannels.Count < 1)
        {
            this.chatClient.JoinChannel(SecretKeys.ChannelName);
        }

        this.chatClient.SendMessage(SecretKeys.ChannelName, message);
        this.ProcessBotMessage(message);
    }

    public int GetCurrentActiveChattersCount()
    {
        return this.currentActiveChatters.Count;
    }
}
