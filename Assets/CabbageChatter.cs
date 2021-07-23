using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwitchLib.Client.Models;
using UnityEngine;

public class CabbageChatter : MonoBehaviour
{
    public GameObject chatCanvasObject;
    public Canvas chatCanvas;
    public GameObject chatBoxObject;
    public RectTransform chatBoxRect;
    public GameObject chatTextObject;
    public GameObject emoteMessageBoxObject;
    public GameObject cabbageVisualHolder;
    public GameObject shooterText;
    public GameObject crown;

    //Character Creator Fields
    public SpriteRenderer headPiece;
    public SpriteRenderer eyeBrows;
    public SpriteRenderer eyes;
    public SpriteRenderer nose;
    public SpriteRenderer mouth;
    public SpriteRenderer baseCabbage;

    public TextMeshProUGUI username;

    public Rigidbody cabbageRigidbody;
    private float minXVelocity = 500f;
    private float maxXVelocity = 1500f;
    private float initialYVelocity = 400f;
    private float launchVelocity = 4000f;

    private Color chatterColor;
    public string chatterName;

    const int MaxChatMessagesVisable = 3;

    private List<string> rerollPhrases;

    private float layerGapAmount = 0.01f;

    private float maxChatboxHeight = 250f;

    private bool shootCooldownActive = false;
    private float shootCooldown = 3.0f;

    public int shootScore = 0;

    [SerializeField]
    private GameObject shootParticleObject;

    private void Awake()
    {
        this.chatterColor = this.GetRandomColor();

        this.rerollPhrases = new List<string>()
        {
            "Behold, my new form!",
            "And this...is to go...EVEN FURTHER BEYOND!",
            "Bleargh, that was ugly.  I look MUCH better now!",
            "AAARRRGGHHH, IT HURRRRRTS!",
            "Did someone call for a sexy new cabbage?",
            "Form of...CABBAGE!",
            "Cower in fear at my new form, mortals!",
            "This is giving me self-esteem issues...",
            "One handome new cabbage, coming right up!",
            "Oh god, is this worse?",
            "Please let this one be okay, this is giving me an existential crisis"
        };
    }

    private void Start()
    {
        this.GenerateCharacter();
    }

    public void ActivateCrown()
    {
        this.crown.SetActive(true);
    }

    public void DeactivateCrown()
    {
        this.crown.SetActive(false);
    }

    private void GenerateCharacter()
    {
        if (this.chatterName.ToLower() == "ruddgasm")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("charles");
            return;
        }
        else if (this.chatterName.ToLower() == "ruddpuddle")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("prisonMike");
            return;
        }
        else if (this.chatterName.ToLower() == "levanter_")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("nicCage");
            return;
        }
        else if (this.chatterName.ToLower() == "coleslawski")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("SuperJaredBall");
            return;
        }
        else if (this.chatterName.ToLower() == "cabbagegatekeeper")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("beefCat");
            return;
        }
        else if (this.chatterName.ToLower() == "roh_ka")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("dampboi");
            return;
        }
        else if (this.chatterName.ToLower() == "safireninja")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("ninjaKanpai");
            return;
        }
        else if (this.chatterName.ToLower() == "nickpea_and_thebean")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("PeaCabbage");
            return;
        }
        else if (this.chatterName.ToLower() == "cotmweasel")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("gengar");
            return;
        }
        /*else if (this.chatterName.ToLower() == "pomothedog")
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("pomo");
            return;
        }*/

        this.headPiece.sprite = CharacterCreator.instance.GetHeadpiece();
        this.eyeBrows.sprite = CharacterCreator.instance.GetEyebrows();
        this.eyes.sprite = CharacterCreator.instance.GetEyes();
        this.nose.sprite = CharacterCreator.instance.GetNose();
        this.mouth.sprite = CharacterCreator.instance.GetMouth();
    }

    public void RerollCharacter()
    {
        this.GenerateCharacter();
        this.DisplayChatMessage(this.chatterName, this.rerollPhrases[Random.Range(0, this.rerollPhrases.Count)]);
    }

    public void UpdateToNewSortingOrder(int order)
    {
        this.chatCanvas.sortingOrder = order;
    }

    public void LaunchAtRandomVelocity()
    {
        float xLaunchDirection = (this.gameObject.transform.position.x > 0) ? -1.0f : 1.0f;
        float xLaunchForce = Random.Range(this.minXVelocity, this.maxXVelocity) * xLaunchDirection;
        this.cabbageRigidbody.AddForce(new Vector3(xLaunchForce, initialYVelocity, 0f));
    }

    public void DisplayChatMessage(string chatterName, string chatMessage)
    {
        this.chatterName = chatterName;

        chatCanvasObject.SetActive(true);

        this.LaunchAtRandomVelocity();


        this.username.color = this.chatterColor;
        this.username.text = chatterName;

        this.shooterText.GetComponent<TextMeshProUGUI>().text = chatterName;

        if (this.chatBoxObject.transform.childCount > MaxChatMessagesVisable)
        {
            this.chatBoxObject.GetComponentInChildren<EmoteMessageBox>().DestroyEarly();
        }

        GameObject newEmoteMessageBoxObject = Instantiate(this.emoteMessageBoxObject, this.chatBoxObject.transform) as GameObject;
        EmoteMessageBox newEmoteMessageBox = newEmoteMessageBoxObject.GetComponent<EmoteMessageBox>();

        newEmoteMessageBox.DisplayMessage(chatMessage);

        //Limit the max height of a chatbox by deleting older messages early when it's getting too big
        if (this.chatBoxRect.rect.height >= this.maxChatboxHeight)
        {
            this.chatBoxObject.GetComponentInChildren<EmoteMessageBox>().DestroyEarly();
        }
    }

    public void ShootCharacter(string direction = "")
    {
        if (this.shootCooldownActive == true)
        {
            return;
        }

        float xLaunchDirection = Random.Range(-1.0f, 1.0f);

        if (direction.ToLower().Contains("left"))
        {
            xLaunchDirection = Random.Range(-1.0f, -0.5f);
        }
        else if (direction.ToLower().Contains("up"))
        {
            xLaunchDirection = Random.Range(-0.4f, 0.4f);
        }
        else if (direction.ToLower().Contains("right"))
        {
            xLaunchDirection = Random.Range(1.0f, 0.5f);
        }

        float yLaunchDirection = Random.Range(0.8f, 1.0f);
        this.cabbageRigidbody.AddForce(new Vector3(xLaunchDirection, yLaunchDirection, 0f) * this.launchVelocity);
        this.shootCooldownActive = true;
        //this.shootParticleObject.SetActive(true);
        //this.shooterText.SetActive(true);
        StartCoroutine(this.ReduceShootCooldown());
        //Invoke("DisableParticleSystem", 1.0f);
    }

    private void DisableParticleSystem()
    {
        this.shootParticleObject.SetActive(false);
    }

    private IEnumerator ReduceShootCooldown()
    {
        yield return new WaitForSeconds(this.shootCooldown);
        //this.shooterText.SetActive(true);
        this.shootCooldownActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bounce") && this.shootCooldownActive)
        {
            Vector3 reflectNormal = collision.GetContact(0).normal;

            if (reflectNormal.x == 0)
            {
                reflectNormal = new Vector3(1.0f, reflectNormal.y, reflectNormal.z);
            }
            if (reflectNormal.y == 0)
            {
                reflectNormal = new Vector3(reflectNormal.x, 1.0f, reflectNormal.z);
            }

            //Debug.LogError("Reflect Normal: " + reflectNormal);

            this.cabbageRigidbody.AddForce(2.0f * new Vector3(this.cabbageRigidbody.velocity.x * (float)reflectNormal.x, this.cabbageRigidbody.velocity.y * (float)reflectNormal.y, this.cabbageRigidbody.velocity.z * (float)reflectNormal.z));
        }

        if (collision.gameObject.CompareTag("Bumper"))
        {
            Vector3 reflectNormal = collision.GetContact(0).normal;
            this.cabbageRigidbody.AddForce(reflectNormal * this.launchVelocity);
            collision.gameObject.GetComponent<AudioSource>().Play();

            if (this.cabbageRigidbody.velocity.magnitude > this.launchVelocity)
            {
                this.cabbageRigidbody.velocity = this.cabbageRigidbody.velocity.normalized * this.launchVelocity;
            }
        }
    }

    public void UpdateLayer(int layerDepth)
    {
        this.cabbageVisualHolder.transform.position = new Vector3(this.cabbageVisualHolder.transform.position.x, 
            this.cabbageVisualHolder.transform.position.y, 
            -layerDepth * this.layerGapAmount);
    }

    private void LateUpdate()
    {
        if (this.chatCanvasObject.activeSelf == true && this.chatBoxObject.transform.childCount == 1)
        {
            this.chatCanvasObject.SetActive(false);
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }

    private void OnDestroy()
    {
        foreach (EmoteMessageBox emoteMessageToDestroy in this.chatBoxObject.GetComponentsInChildren<EmoteMessageBox>())
        {
            emoteMessageToDestroy.DestroyEarly();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "kill")
        {
            ChatManager.instance.RemoveCabbage(this.username.text);
        }
    }
}
