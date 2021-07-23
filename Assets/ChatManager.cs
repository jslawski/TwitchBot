using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;
using UnityEngine.UI;
using UnityRawInput;
using TwitchLib.Communication.Models;
using System;
using TwitchLib.Communication.Clients;
using System.Collections;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    public Client chatClient;
    private ConnectionCredentials botCreds = new ConnectionCredentials(TwitchSecrets.BotName, TwitchSecrets.AccessToken);

    private PubSub pubSubClient;

    public GameObject parentChat;
    public GameObject cabbageChatterPrefab;
    public BoxCollider spawnBoundaries;

    public ParticleSystem shotsHype;
    public AudioSource shotsAudio;

    public Dictionary<string, CabbageChatter> chatterDict;
    public Dictionary<string, int> chatterScoreHistory;

    Dictionary<string, int> wheelWeights;
    public DrinkWheel currentDrinkWheel;

    public List<CabbageChatter> currentActiveChatters;

    public Queue<CabbageChatter> chatterQueue;
    private bool readyForNextChatter = true;

    [SerializeField]
    private EndingSetup endingSetup;

    public bool killSwitchActive = false;

    [SerializeField]
    private GameObject alwaysSunnyPanel;
    [SerializeField]
    private Text alwaysSunnyText;

    static public bool shootModeActive = false;
    [SerializeField]
    private PhysicMaterial bounceMaterial;
    [SerializeField]
    private GameObject hoopObject;
    [SerializeField]
    private GameObject leaderboardCanvas;
    [SerializeField]
    private AudioSource bballSource;
    private AudioClip[] bballHoopMusic;
    private GameObject[] bballLevels;

    private const string ClipStub = "https://clips.twitch.tv/";
    private string recentClip = string.Empty;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Application.runInBackground = true;

        ClientOptions clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new WebSocketClient(clientOptions);

        this.chatClient = new Client();
        this.chatClient.Initialize(botCreds, TwitchSecrets.ChannelName);
        this.chatClient.OnMessageReceived += this.ProcessMessage;
        this.chatClient.WillReplaceEmotes = true;
        this.chatClient.OnUserLeft += this.RemoveCabbageChatter;
        this.chatClient.OnChatCommandReceived += this.ProcessCommand;
        this.chatClient.OnConnected += this.Client_OnConnected;
        this.chatClient.AutoReListenOnException = true;
        this.chatClient.Disconnect();
        this.chatClient.Connect();

        this.pubSubClient = new PubSub();
        this.pubSubClient.OnPubSubServiceConnected += this.PubSubConnected;
        this.pubSubClient.OnRewardRedeemed += this.PubSubRewardRedeemed;
        this.pubSubClient.Disconnect();
        this.pubSubClient.Connect();

        this.chatterDict = new Dictionary<string, CabbageChatter>();
        this.chatterScoreHistory = new Dictionary<string, int>();
        this.currentActiveChatters = new List<CabbageChatter>();
        this.chatterQueue = new Queue<CabbageChatter>();

        SetupWheelDict();

        if (!Application.isEditor)
        {
            this.bballHoopMusic = Resources.LoadAll<AudioClip>("BBallMusic");
        }

        if (!Application.isEditor)
        {
            RawKeyInput.Start(true);
            RawKeyInput.OnKeyUp += HandleKeyUp;
        }

        this.bballLevels = Resources.LoadAll<GameObject>("BBallLevels");

        StartCoroutine(this.RejoinHeartbeat());
    }

    private void HandleKeyUp(RawKey key)
    {
        if (key == RawKey.F7)
        {
            this.ShowRecentClip();
        }
        else if (key == RawKey.F2)
        {
            this.ToggleShootMode();
        }

        else if (key == RawKey.F3)
        {
            this.LaunchAllCabbages();
        }
    }

    private void PubSubConnected(object sender, System.EventArgs e)
    {
        pubSubClient.ListenToRewards(TwitchSecrets.ChannelID);
        pubSubClient.SendTopics();
    }

    private void PubSubRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Debug.LogError("Reward ID: " + e.RewardId.ToString());

        if (e.RewardId.ToString() == TwitchSecrets.AlcoholUpRewardID)
        {
            this.UpdateWheel(e.Message.ToLower(), 1);
        }
        else if (e.RewardId.ToString() == TwitchSecrets.AlcoholDownRewardID)
        {
            this.UpdateWheel(e.Message.ToLower(), -1);
        }

        if (this.killSwitchActive)
        {
            return;
        }

        if (e.RewardId.ToString() == TwitchSecrets.ShotsRewardID)
        {
            this.InitiateShotsHype();
        }
        else if (e.RewardId.ToString() == TwitchSecrets.AlwaysSunnyRewardID)
        {
            this.InitiateAlwaysSunny(e.Message);
        }
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        this.chatClient.JoinChannel(TwitchSecrets.ChannelName, true);
    }

    private void ProcessCommand(object sender, OnChatCommandReceivedArgs e)
    {
        if (e.Command.ChatMessage.Username == "cabbagegatekeeper")
        {
            if (e.Command.CommandText.ToLower().Contains("oceanman"))
            {
                this.endingSetup.BeginEndSequence();
            }

            if (e.Command.CommandText.ToLower().Contains("killswitch"))
            {
                this.killSwitchActive = !this.killSwitchActive;
            }
        }

        if (this.killSwitchActive)
        {
            return;
        }

        if (e.Command.ChatMessage.Username == "coleslawski")
        {
            if (e.Command.CommandText.ToLower().Contains("roll"))
            {
                this.RollCall();
            }

            if (e.Command.CommandText.ToLower().Contains("bball"))
            {
                this.ToggleShootMode();
            }

            if (e.Command.CommandText.ToLower().Contains("fire"))
            {
                this.LaunchAllCabbages();
            }
        }

        if (e.Command.ChatMessage.UserId == "51114479" && e.Command.CommandText.ToLower().Contains("shots"))
        {
            this.InitiateShotsHype();
        }

        if (this.chatterDict.ContainsKey(e.Command.ChatMessage.Username) && e.Command.CommandText.ToLower().Contains("cabbage"))
        {
            this.chatterDict[e.Command.ChatMessage.Username].RerollCharacter();
        }

        if (e.Command.CommandText.ToLower().Contains("shoot") && shootModeActive == true)
        {
            if (this.chatterDict.ContainsKey(e.Command.ChatMessage.Username))
            {
                string chatUsername = e.Command.ChatMessage.Username.ToLower();
                if (chatUsername == "coleslawski" || chatUsername == "ruddgasm" || chatUsername == "ruddpuddle" || chatUsername == "levanter_")
                {
                    StartCoroutine(DelayedShot(e.Command.ChatMessage.Username, e.Command.ArgumentsAsString));
                }
                else
                {
                    this.chatterDict[e.Command.ChatMessage.Username].ShootCharacter(e.Command.ArgumentsAsString);
                }
            }
            else
            {
                this.SpawnNewChatter(e.Command.ChatMessage);
                this.chatterDict[e.Command.ChatMessage.Username].ShootCharacter(e.Command.ArgumentsAsString);
            }
        }
    }

    private IEnumerator DelayedShot(string username, string direction)
    {
        float randomDelay = UnityEngine.Random.Range(1.0f, 3.5f);

        yield return new WaitForSeconds(randomDelay);

        this.chatterDict[username].ShootCharacter(direction);
    }

    private void ProcessMessage(object sender, OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Username == "soundalerts")
        {
            return;
        }

        if (e.ChatMessage.Message.StartsWith("!"))
        {
            return;
        }

        if (e.ChatMessage.Message.Contains(ChatManager.ClipStub))
        {
            this.GrabClipLink(e.ChatMessage.Message);
        }

        if (this.chatterDict.ContainsKey(e.ChatMessage.Username))
        {
            this.chatterDict[e.ChatMessage.Username].DisplayChatMessage(e.ChatMessage.Username, this.GetProperMessage(e.ChatMessage));
            this.chatterQueue.Enqueue(this.chatterDict[e.ChatMessage.Username]);
        }
        else //Create a new cabbage chatter
        {
            this.SpawnNewChatter(e.ChatMessage);
        }
    }

    private void SpawnNewChatter(ChatMessage newChatterMessage)
    {
        float randomXPosition = UnityEngine.Random.Range(spawnBoundaries.bounds.min.x, spawnBoundaries.bounds.max.x);
        Vector3 instantiationPosition = new Vector3(randomXPosition, spawnBoundaries.transform.position.y, 0f);
        GameObject newChatter = Instantiate(cabbageChatterPrefab, instantiationPosition, new Quaternion(), this.parentChat.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        this.chatterDict.Add(newChatterMessage.Username, cabbageChatter);
        this.currentActiveChatters.Add(cabbageChatter);
        this.chatterQueue.Enqueue(cabbageChatter);

        cabbageChatter.chatterName = newChatterMessage.Username.ToLower();
        cabbageChatter.DisplayChatMessage(newChatterMessage.Username, this.GetProperMessage(newChatterMessage));
        newChatter.name = newChatterMessage.Username;

        //Update chatter with their last shoot score, if it exists
        //Otherwise, initialize it to 0
        if (this.chatterScoreHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.shootScore = this.chatterScoreHistory[cabbageChatter.chatterName];

            //Toggle crown if the leader has respawned
            if (Leaderboard.instance.topLeaders[0].username.text == cabbageChatter.username.text)
            {
                cabbageChatter.ActivateCrown();
            }
        }
        else
        {
            this.chatterScoreHistory.Add(cabbageChatter.chatterName, 0);
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

    private void RemoveCabbageChatter(object sender, OnUserLeftArgs e)
    {
        this.RemoveCabbage(e.Username);
    }

    public void RemoveCabbage(string username)
    {
        if (this.chatterDict.ContainsKey(username))
        {
            Destroy(this.chatterDict[username].gameObject);
            currentActiveChatters.Remove(this.chatterDict[username]);
            this.chatterDict.Remove(username);
        }
    }

    private void PushChatterToFront(CabbageChatter latestChatter)
    {
        this.readyForNextChatter = false;
        //Remove the chatter from it's current position in the list, and put it at the end
        this.currentActiveChatters.Remove(latestChatter);
        this.currentActiveChatters.Add(latestChatter);
        UpdateChatterLayers();
        this.readyForNextChatter = true;
    }

    private void UpdateChatterLayers()
    {
        for (int i = 0; i < this.currentActiveChatters.Count; i++)
        {
            this.currentActiveChatters[i].UpdateLayer(i);
        }
    }

    private void InitiateShotsHype()
    {
        this.shotsHype.Play();
        this.shotsAudio.Play();
    }

    private string GetWheelKey(string wheelValue)
    {
        string wheelKey = string.Empty;

        foreach (KeyValuePair<string, int> entry in wheelWeights)
        {
            if (entry.Key.ToLower().Contains(wheelValue))
            {
                wheelKey = entry.Key;
                break;
            }
        }

        return wheelKey;
    }

    private void UpdateWheel(string wheelValue, int change)
    {
        //Find the corresponding wheel value
        string selectedKey = GetWheelKey(wheelValue);
        RedeemWheelReward(selectedKey, change);
    }

    private void SetupWheelDict()
    {
        wheelWeights = new Dictionary<string, int>();
        wheelWeights.Add("Vodka/Seductive", 5);
        wheelWeights.Add("Midori/JarJar", 5);
        wheelWeights.Add("Tequila/Wizened", 5);
        wheelWeights.Add("Jager/Surfer", 5);
        wheelWeights.Add("Sake/NYBaby", 5);
        wheelWeights.Add("SoCo/Jammer", 5);
        wheelWeights.Add("Gin/Deep", 5);
        wheelWeights.Add("Whiskey/Influencer", 5);
        wheelWeights.Add("Rum/Scottish", 5);

        this.currentDrinkWheel.SetupColorDict();
        this.currentDrinkWheel.UpdateValues(wheelWeights);
    }

    private void RedeemWheelReward(string wheelTarget, int change)
    {
        if (wheelWeights[wheelTarget] <= 0 || wheelWeights[wheelTarget] >= 20)
        {
            return;
        }

        wheelWeights[wheelTarget] += change;
        this.currentDrinkWheel.UpdateValues(wheelWeights);
    }

    private void InitiateAlwaysSunny(string title)
    {
        this.alwaysSunnyText.text = "\"" + title + "\"";
        this.alwaysSunnyPanel.SetActive(true);
        Invoke("DeactivateAlwaysSunny", 7.5f);
    }

    private void DeactivateAlwaysSunny()
    {
        this.alwaysSunnyPanel.SetActive(false);
    }

    private void RollCall()
    {
        foreach (CabbageChatter activeChatter in this.parentChat.GetComponentsInChildren<CabbageChatter>())
        {
            activeChatter.DisplayChatMessage(activeChatter.username.text, "Hi, I'm " + activeChatter.username.text + "!");
        }
    }

    private void ToggleShootMode()
    {
        shootModeActive = !shootModeActive;
        //this.hoopObject.SetActive(shootModeActive);

        int levelIndex = 1;//UnityEngine.Random.Range(0, this.bballLevels.Length);
        GameObject randomLevel = this.bballLevels[levelIndex];
        Instantiate(randomLevel, this.hoopObject.transform, false);

        this.leaderboardCanvas.SetActive(shootModeActive);

        if (shootModeActive)
        {
            if (!Application.isEditor)
            {
                int audioClipIndex = UnityEngine.Random.Range(0, this.bballHoopMusic.Length);
                this.bballSource.clip = this.bballHoopMusic[audioClipIndex];
                bballSource.Play();
            }

            this.bounceMaterial.bounciness = 0.75f;
        }
        else
        {
            this.bballSource.Stop();
            this.bounceMaterial.bounciness = 0.5f;
        }
    }

    private void LaunchAllCabbages()
    {
        foreach (CabbageChatter chatter in this.currentActiveChatters)
        {
            chatter.ShootCharacter();
        }
    }

    private void GrabClipLink(string chatMessage)
    {
        int startIndex = chatMessage.IndexOf(ChatManager.ClipStub);
        string remainingMessage = chatMessage.Substring(startIndex);
        int endIndex = remainingMessage.IndexOf(" ");
        if (endIndex < 0)
        {
            endIndex = remainingMessage.Length;
        }

        this.recentClip = chatMessage.Substring(startIndex, endIndex);
    }

    private void ShowRecentClip()
    {
        //Debug.LogError("Recent Clip: " + this.recentClip);
        this.chatClient.SendMessage(TwitchSecrets.ChannelName, "!showClip " + this.recentClip);
    }

    private void Update()
    {
        if (this.chatterQueue.Count > 0 && this.readyForNextChatter == true)
        {
            this.PushChatterToFront(this.chatterQueue.Dequeue());
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F2))
        {
            this.ToggleShootMode();
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F7))
        {
            this.ShowRecentClip();
        }
    }

    private IEnumerator RejoinHeartbeat()
    {
        while (true)
        {
            this.chatClient.JoinChannel(TwitchSecrets.ChannelName, true);
            yield return new WaitForSeconds(300);
        }
    }

    private void OnApplicationQuit()
    {
        this.chatClient.Disconnect();

        if (!Application.isEditor)
        {
            RawKeyInput.Stop();
        }
    }
}
