using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Pretty sure this is a useless class. DELETE IT!

public class CabbagePlinko : MonoBehaviour
{
    public GameObject crown;
    public GameObject prestigeBadge;
    public TextMeshProUGUI prestigeText;
    public GameObject magnifyingGlass;

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
    private float nukeVelocity = 7000f;
    private float arcForce = 0.15f;

    private Color chatterColor;
    public string chatterName;

    const int MaxChatMessagesVisable = 3;

    private List<string> rerollPhrases;
    private List<string> hmmPhrases;

    private float layerGapAmount = 0.01f;

    private float maxChatboxHeight = 250f;

    private bool shootCooldownActive = false;
    private float shootCooldown = 3.0f;

    public int shootScore = 0;

    public int prestigeLevel = 0;

    [SerializeField]
    private GameObject prestigeAnnouncement;

    private float prestigeAnnouncementMinX = -18f;
    private float prestigeAnnouncementMaxX = 18f;
    private float prestigeAnnouncementMinY = -5f;
    private float prestigeAnnouncementMaxY = 7f;

    [SerializeField]
    private GameObject shootParticleObject;

    private static float spawnDepth = -2.0f;

    [SerializeField]
    private GameObject deathEffect;

    private void Awake()
    {
        this.chatterColor = this.GetRandomColor();
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

    public void UpdatePrestigeBadge()
    {
        this.prestigeBadge.SetActive(true);
        this.prestigeText.text = this.prestigeLevel.ToString();
    }

    
    private void GenerateCharacter()
    {
        
        this.username.text = this.chatterName;
        this.username.color = this.chatterColor;

        //StartCoroutine(this.LoadUserData());
        
        /*
        if (ChatManager.instance.mjTime == true)
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("CustomCabbages/michaelJordan");
            return;
        }

        if (ChatManager.instance.customCabbageSpriteNames.Contains(this.chatterName.ToLower()))
        {
            this.baseCabbage.sprite = Resources.Load<Sprite>("CustomCabbages/" + this.chatterName.ToLower());
            return;
        }

        this.headPiece.sprite = CharacterCreator.instance.GetHeadpiece();
        this.eyeBrows.sprite = CharacterCreator.instance.GetEyebrows();
        this.eyes.sprite = CharacterCreator.instance.GetEyes();
        this.nose.sprite = CharacterCreator.instance.GetNose();
        this.mouth.sprite = CharacterCreator.instance.GetMouth();
        */
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

            this.gameObject.layer = LayerMask.NameToLayer("launchedCabbage");
        }

        if (collision.gameObject.CompareTag("floor"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("cabbage");
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }

    public void TriggerPrestige()
    {
        this.prestigeLevel++;
        this.shootScore = 0;
        this.UpdatePrestigeBadge();

        float randomX = Random.Range(this.prestigeAnnouncementMinX, this.prestigeAnnouncementMaxX);
        float randomY = Random.Range(this.prestigeAnnouncementMinY, this.prestigeAnnouncementMaxY);

        Vector3 spawnPoint = new Vector3(randomX, randomY, CabbageChatter.spawnDepth);

        GameObject prestigeObject = Instantiate(this.prestigeAnnouncement, spawnPoint, new Quaternion());
        PrestigeAnimation prestigeComponent = prestigeObject.GetComponent<PrestigeAnimation>();
        prestigeComponent.SetCabbage(this.baseCabbage, this.headPiece, this.eyeBrows, this.eyes, this.nose, this.mouth, this.prestigeLevel); ;

        CabbageChatter.spawnDepth -= 0.01f;
    }
}
