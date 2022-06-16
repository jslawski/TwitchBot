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
    //TODO: Get rid of this.  Keep track of total score and do prestige/leaderboard calculations
    //      based on the total score % 10
    public Dictionary<string, int> chatterPrestigeHistory;

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
    private GameObject[] bballLevels;
    private GameObject activeBBallLevel;

    public CabbageChatter lastLeader;

    private const string ClipStub = "https://clips.twitch.tv/";
    private string recentClip = string.Empty;

    private List<string> cabbageCodeVictors;

    public bool mjTime = false;

    public List<string> customCabbageSpriteNames;

    public bool plinko = false;
    private GameObject[] plinkoLevels;
    private GameObject activePlinkoLevel;
    [SerializeField]
    private GameObject plinkoObject;
    public GameObject parentPlinko;
    private int currentPlinkoLevelIndex = -1;

    private List<int> seenPlinkoLevels;
    private List<int> unseenPlinkoLevels;

    [SerializeField]
    private GameObject floorCollider;
    [SerializeField]
    private GameObject destroyCollider;
    [SerializeField]
    private GameObject leftWallCollider;
    [SerializeField]
    private AudioSource plinkoSound;

    private Coroutine aiCoroutine = null;
    private float secondsBetweenAIShots = 15.0f;

    public int prestigeThreshold = 50;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
        
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
        this.chatterPrestigeHistory = new Dictionary<string, int>();
        this.currentActiveChatters = new List<CabbageChatter>();
        this.chatterQueue = new Queue<CabbageChatter>();

        SetupWheelDict();

        if (!Application.isEditor)
        {
            RawKeyInput.Start(true);
            RawKeyInput.OnKeyUp += HandleKeyUp;
        }
        
        this.bballLevels = Resources.LoadAll<GameObject>("BBallLevels");
        this.plinkoLevels = Resources.LoadAll<GameObject>("PlinkoLevels");

        this.seenPlinkoLevels = new List<int>();
        this.unseenPlinkoLevels = new List<int>();
        for (int i = 0; i < this.plinkoLevels.Length; i++)
        {
            this.unseenPlinkoLevels.Add(i);
        }

        this.InitializeCabbageCodeVictors();

        StartCoroutine(this.RejoinHeartbeat());
    }

    private void Start()
    {
        this.CreateCustomCabbageList();
    }

    private void CreateCustomCabbageList()
    {
        this.customCabbageSpriteNames = new List<string>();
        this.customCabbageSpriteNames.Add("ruddgasm");
        this.customCabbageSpriteNames.Add("ruddpuddle");
        this.customCabbageSpriteNames.Add("levanter_");
        this.customCabbageSpriteNames.Add("coleslawski");
        this.customCabbageSpriteNames.Add("cabbagegatekeeper");
        this.customCabbageSpriteNames.Add("roh_ka");
        this.customCabbageSpriteNames.Add("safireninja");
        this.customCabbageSpriteNames.Add("nickpea_and_thebean");
        this.customCabbageSpriteNames.Add("cotmweasel");
        this.customCabbageSpriteNames.Add("pomothedog");
        this.customCabbageSpriteNames.Add("itsboats");
        this.customCabbageSpriteNames.Add("rookrules");
        this.customCabbageSpriteNames.Add("doctor_denny");
        this.customCabbageSpriteNames.Add("dalbusg");
        this.customCabbageSpriteNames.Add("mastyff");
        this.customCabbageSpriteNames.Add("chrisvsbacklog");
        this.customCabbageSpriteNames.Add("thejackalanimatronic");
    }

    private void HandleKeyUp(RawKey key)
    {
        if (key == RawKey.F7)
        {
            this.ShowRecentClip();
        }
        else if (key == RawKey.F2)
        {
            this.TogglePlinko();
        }

        else if (key == RawKey.F3)
        {
            this.ToggleShootMode();
        }

        else if (key == RawKey.F4)
        {
            if (this.plinko)
            {
                this.JostleAllCabbages();
            }
            else
            {
                this.LaunchAllCabbages();
            }
        }

        else if (key == RawKey.F12)
        {
            this.SendPlsLaugh();
        }

        if (key == RawKey.M && shootModeActive)
        {
            if (this.bballSource.isPlaying)
            {
                this.bballSource.Stop();
            }
            else
            {
                this.bballSource.Play();    
            }
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
        else if (e.RewardId.ToString() == TwitchSecrets.NukeCabbageRewardID)
        {
            this.NukeCabbage(e.DisplayName, e.Message);
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

            if (e.Command.CommandText.ToLower().Contains("nuke"))
            {
                this.NukeCabbage("coleslawski", e.Command.ArgumentsAsString);
            }

            if (e.Command.CommandText.ToLower().Contains("slam"))
            {
                this.ToggleMJ();
            }
        }

        if ((e.Command.ChatMessage.Username.ToLower() == "safireninja" || e.Command.ChatMessage.Username.ToLower() == "coleslawski") && e.Command.CommandText.ToLower().Contains("shot"))
        {
            this.InitiateShotsHype();
        }

        if (this.chatterDict.ContainsKey(e.Command.ChatMessage.Username.ToLower()) && e.Command.CommandText.ToLower().Contains("cabbage"))
        {
            this.chatterDict[e.Command.ChatMessage.Username.ToLower()].RerollCharacter();
        }

        if (e.Command.CommandText.ToLower().Contains("shoot") && shootModeActive == true)
        {
            if (this.chatterDict.ContainsKey(e.Command.ChatMessage.Username.ToLower()))
            {
                string chatUsername = e.Command.ChatMessage.Username.ToLower();
                if (chatUsername == "coleslawski" || chatUsername == "ruddgasm" || chatUsername == "ruddpuddle" || chatUsername == "levanter_")
                {
                    StartCoroutine(DelayedShot(e.Command.ChatMessage.Username, e.Command.ArgumentsAsString));
                }
                else
                {
                    this.chatterDict[e.Command.ChatMessage.Username.ToLower()].ShootCharacter(e.Command.ArgumentsAsString);
                }
            }
            else
            {
                this.SpawnNewChatter(e.Command.ChatMessage.Username.ToLower(), e.Command.ChatMessage);
                this.chatterDict[e.Command.ChatMessage.Username.ToLower()].ShootCharacter(e.Command.ArgumentsAsString);
            }
        }

        if (e.Command.CommandText.ToLower().Contains("hmm"))
        {
            this.ActivateHmmCommand(e.Command.ChatMessage);
        }

        if (e.Command.CommandText.ToLower().Contains("rand"))
        {
            GameObject dropZonesParentObject = GameObject.Find("DropZonesParent");

            if (dropZonesParentObject != null)
            {
                this.AttemptPlinkoDrop(e.Command.ChatMessage.Username.ToLower(),
                    UnityEngine.Random.Range(1, dropZonesParentObject.transform.childCount + 1));
            }
        }

        int plinkoResult = 0;
        if (this.plinko == true && int.TryParse(e.Command.CommandText, out plinkoResult))
        {
            this.AttemptPlinkoDrop(e.Command.ChatMessage.Username.ToLower(), plinkoResult);
        }
    }

    private IEnumerator DelayedShot(string username, string direction)
    {
        float randomDelay = UnityEngine.Random.Range(1.0f, 2.5f);

        yield return new WaitForSeconds(randomDelay);

        this.chatterDict[username.ToLower()].ShootCharacter(direction);
    }

    private void ProcessMessage(object sender, OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Username == "soundalerts")
        {
            return;
        }

        if (e.ChatMessage.Message.StartsWith("!") && e.ChatMessage.Username == "cabbagegatekeeper")
        {            
            return;
        }

        if (e.ChatMessage.Message.Contains(ChatManager.ClipStub))
        {
            this.GrabClipLink(e.ChatMessage.Message);
        }

        if (this.chatterDict.ContainsKey(e.ChatMessage.Username.ToLower()))
        {
            this.chatterDict[e.ChatMessage.Username.ToLower()].DisplayChatMessage(e.ChatMessage.Username, this.GetProperMessage(e.ChatMessage));
            this.chatterQueue.Enqueue(this.chatterDict[e.ChatMessage.Username.ToLower()]);
        }
        else if (this.plinko == false) //Create a new cabbage chatter
        {
            this.SpawnNewChatter(e.ChatMessage.Username.ToLower(), e.ChatMessage);
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

        if (newChatterMessage != null)
        {
            cabbageChatter.DisplayChatMessage(username, this.GetProperMessage(newChatterMessage));
        }

        //Update chatter with their last shoot score, if it exists
        //Otherwise, initialize it to 0
        if (this.chatterScoreHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.shootScore = this.chatterScoreHistory[cabbageChatter.chatterName];

            //Toggle crown if the leader has respawned
            if (Leaderboard.instance.topLeaders[0].username.text == cabbageChatter.chatterName.ToLower())
            {
                cabbageChatter.ActivateCrown();
            }
        }
        else
        {
            this.chatterScoreHistory.Add(cabbageChatter.chatterName, 0);
        }

        //Do the same thing with prestige
        if (this.chatterPrestigeHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.prestigeLevel = this.chatterPrestigeHistory[cabbageChatter.chatterName];

            //Toggle prestige badge if player has one
            if (cabbageChatter.prestigeLevel > 0)
            {
                cabbageChatter.UpdatePrestigeBadge();
            }
        }
        else
        {
            this.chatterPrestigeHistory.Add(cabbageChatter.chatterName, 0);
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
        if (this.chatterDict.ContainsKey(username.ToLower()))
        {
            Destroy(this.chatterDict[username.ToLower()].gameObject);
            currentActiveChatters.Remove(this.chatterDict[username.ToLower()]);
            this.chatterDict.Remove(username.ToLower());            
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

        int levelIndex = UnityEngine.Random.Range(0, this.bballLevels.Length);
        GameObject randomLevel = this.bballLevels[levelIndex];

        if (shootModeActive)
        {
            this.activeBBallLevel = Instantiate(randomLevel, this.hoopObject.transform, false) as GameObject;
            AutoScore.SetHoopTransform(GameObject.Find("bballhoop").transform);

            if (this.aiCoroutine == null)
            {
                this.aiCoroutine = StartCoroutine(this.RunAI());
            }
        }
        else
        {
            Destroy(this.activeBBallLevel);
        }

        this.leaderboardCanvas.SetActive(shootModeActive);

        if (shootModeActive)
        {
            /*if (!Application.isEditor)
            {
                int audioClipIndex = UnityEngine.Random.Range(0, this.bballHoopMusic.Length);
                this.bballSource.clip = this.bballHoopMusic[audioClipIndex];
                bballSource.Play();
            }
            */
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

    private void JostleAllCabbages()
    {
        foreach (CabbageChatter chatter in this.currentActiveChatters)
        {
            chatter.LaunchAtRandomVelocity();
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

    private void SendPlsLaugh()
    {
        this.chatClient.SendMessage(TwitchSecrets.ChannelName, "!plslaugh");
    }

    private void InitializeCabbageCodeVictors()
    {
        this.cabbageCodeVictors = new List<string> { "coleslawski", "ruddgasm", "ruddpuddle", "brainoidgames", "pomothedog", "spacey3d", "johngames", "roh_ka", "nickpea_and_thebean", "rookrules", "doctor_denny", "honestdangames" };
    }

    private void ActivateHmmCommand(ChatMessage activateMessage)
    {
        string chatUsername = activateMessage.Username.ToLower();

        if (this.cabbageCodeVictors.Contains(chatUsername))
        {
            if (this.chatterDict.ContainsKey(chatUsername.ToLower()))
            {
                    this.chatterDict[chatUsername.ToLower()].ToggleMagnifyingGlass();
            }
            else
            {
                this.SpawnNewChatter(activateMessage.Username.ToLower(), activateMessage);
                this.chatterDict[chatUsername.ToLower()].ToggleMagnifyingGlass();
            }
        }
        else
        {
            this.chatClient.SendMessage(TwitchSecrets.ChannelName, "@" + chatUsername + " ACCESS DENIED! If you would access to this command, then you must solve the Cabbage Code at: https://jared-slawski.itch.io/the-cabbage-code");
        }
    }

    private void NukeCabbage(string attacker, string target)
    {
        string targetNoAt = target;
        if (target.Contains("@"))
        {
            targetNoAt = target.Substring(1).ToLower();
            if (targetNoAt.Contains(" "))
            {
                targetNoAt = targetNoAt.Remove(targetNoAt.Length - 1);
            }
        }

        if (this.chatterDict.ContainsKey(targetNoAt))
        {
            this.chatterDict[targetNoAt].NukeCabbage();

            if (attacker == targetNoAt)
            {
                this.chatClient.SendMessage(TwitchSecrets.ChannelName, "@" + attacker + " nuked themselves!");
            }
            else
            {
                this.chatClient.SendMessage(TwitchSecrets.ChannelName, "@" + attacker + " nuked " + target);
            }
        } 
    }

    private void ToggleMJ()
    {
        foreach (CabbageChatter chatter in this.currentActiveChatters)
        {
            chatter.NukeCabbage();
        }

        this.mjTime = !this.mjTime;
    }

    private void TogglePlinko()
    {
        this.plinko = !this.plinko;

        this.leaderboardCanvas.SetActive(this.plinko);

        if (this.plinko == true)
        {
            this.floorCollider.SetActive(false);
            this.leftWallCollider.SetActive(false);
            this.destroyCollider.SetActive(true);
            this.plinkoSound.Play();

            StartCoroutine(this.ActivatePlinkoLevel());
            
        }
        else
        {
            StartCoroutine(this.CleanupPlinko());
        }
    }

    public void LoadNewPlinkoLevel()
    {
        StartCoroutine(ActivatePlinkoLevel());
    }

    private IEnumerator ActivatePlinkoLevel()
    {
        if (this.aiCoroutine == null)
        {
            this.aiCoroutine = StartCoroutine(this.RunAI());
        }

        if (this.activePlinkoLevel != null)
        {
            Destroy(this.activePlinkoLevel.gameObject);
            yield return null;
        }

        while (this.chatterDict.Count > 0)
        {
            yield return null;
        }

        /*if (this.currentPlinkoLevelIndex == -1)
        {
            this.currentPlinkoLevelIndex = 0;
        }
        else
        {
            int newPlinkoLevelIndex = this.currentPlinkoLevelIndex;

            while (newPlinkoLevelIndex == this.currentPlinkoLevelIndex)
            {
                newPlinkoLevelIndex = UnityEngine.Random.Range(0, this.plinkoLevels.Length);
            }
            
            this.currentPlinkoLevelIndex = (this.currentPlinkoLevelIndex + 1) % this.plinkoLevels.Length;
        }*/

        this.SelectNewPlinkoLevel();

        GameObject randomLevel = this.plinkoLevels[this.currentPlinkoLevelIndex];

        this.activePlinkoLevel = Instantiate(randomLevel, this.plinkoObject.transform, false) as GameObject;
    }

    private void SelectNewPlinkoLevel()
    {
        this.currentPlinkoLevelIndex = this.unseenPlinkoLevels[UnityEngine.Random.Range(0, this.unseenPlinkoLevels.Count)];

        this.seenPlinkoLevels.Add(this.currentPlinkoLevelIndex);
        this.unseenPlinkoLevels.Remove(this.currentPlinkoLevelIndex);

        if (this.unseenPlinkoLevels.Count == 0)
        {
            this.unseenPlinkoLevels = this.seenPlinkoLevels;
            this.seenPlinkoLevels = new List<int>();
        }        
    }

    private IEnumerator CleanupPlinko()
    {
        Destroy(this.activePlinkoLevel.gameObject);

        while (this.chatterDict.Count > 0)
        {
            yield return null;
        }

        this.floorCollider.SetActive(true);
        this.leftWallCollider.SetActive(true);
        this.destroyCollider.SetActive(false);
    }

    public void AttemptPlinkoDrop(string username, int dropIndex)
    {
        PlinkoLevel currentPlinkoLevel = this.activePlinkoLevel.GetComponent<PlinkoLevel>();

        if (currentPlinkoLevel == null || this.chatterDict.ContainsKey(username) || !currentPlinkoLevel.IsValidDropIndex(dropIndex))
        {
            return;
        }

        GameObject newChatter = Instantiate(cabbageChatterPrefab, Vector3.zero, new Quaternion(), this.parentPlinko.transform) as GameObject;
        CabbageChatter cabbageChatter = newChatter.GetComponent<CabbageChatter>();
        this.chatterDict.Add(username, cabbageChatter);
        //Debug.LogError("New Chatter Dict Key: " + username);
        this.currentActiveChatters.Add(cabbageChatter);

        cabbageChatter.chatterName = username.ToLower();
        newChatter.name = username;

        //Update chatter with their last shoot score, if it exists
        //Otherwise, initialize it to 0
        if (this.chatterScoreHistory.ContainsKey(cabbageChatter.chatterName.ToLower()))
        {
            cabbageChatter.shootScore = this.chatterScoreHistory[cabbageChatter.chatterName.ToLower()];

            //Toggle crown if the leader has respawned
            if (Leaderboard.instance.topLeaders[0].username.text == cabbageChatter.chatterName.ToLower())
            {
                cabbageChatter.ActivateCrown();
            }
        }
        else
        {
            this.chatterScoreHistory.Add(cabbageChatter.chatterName.ToLower(), 0);
        }

        //Do the same thing with prestige
        if (this.chatterPrestigeHistory.ContainsKey(cabbageChatter.chatterName))
        {
            cabbageChatter.prestigeLevel = this.chatterPrestigeHistory[cabbageChatter.chatterName];

            //Toggle prestige badge if player has one
            if (cabbageChatter.prestigeLevel > 0)
            {
                cabbageChatter.UpdatePrestigeBadge();
            }
        }
        else
        {
            this.chatterPrestigeHistory.Add(cabbageChatter.chatterName, 0);
        }

        cabbageChatter.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        currentPlinkoLevel.ProcessDropCommand(cabbageChatter, dropIndex);
    }

    private void Update()
    {
        if (this.chatterQueue.Count > 0 && this.readyForNextChatter == true)
        {
            this.PushChatterToFront(this.chatterQueue.Dequeue());
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F2))
        {
            this.TogglePlinko();
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F7))
        {
            this.ShowRecentClip();
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F3))
        {
            this.ToggleShootMode();
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F4))
        {
            if (this.plinko)
            {
                this.JostleAllCabbages();
            }
            else
            {
                this.LaunchAllCabbages();
            }
        }

        if (Application.isEditor && Input.GetKeyUp(KeyCode.F12))
        {
            this.SendPlsLaugh();
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

    private IEnumerator RunAI()
    {
        yield return new WaitForSeconds(this.secondsBetweenAIShots);

        while (this.plinko || shootModeActive)
        {
            if (this.plinko == true)
            {
                GameObject dropZonesParentObject = GameObject.Find("DropZonesParent");

                if (dropZonesParentObject != null)
                {
                    this.AttemptPlinkoDrop("cabbagegatekeeper",
                        UnityEngine.Random.Range(1, dropZonesParentObject.transform.childCount + 1));
                }

            }
            else if (shootModeActive)
            {
                if (this.chatterDict.ContainsKey("cabbagegatekeeper"))
                {
                    this.chatterDict["cabbagegatekeeper"].ShootCharacter();
                }
            }

            yield return new WaitForSeconds(this.secondsBetweenAIShots);
        }

        this.aiCoroutine = null;
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
