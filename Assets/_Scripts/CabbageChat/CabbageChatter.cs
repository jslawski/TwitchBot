﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class CabbageChatter : MonoBehaviour
{
    public GameObject chatCanvasObject;
    public Canvas chatCanvas;
    public GameObject chatBoxObject;
    public RectTransform chatBoxRect;
    public GameObject chatTextObject;
    public GameObject emoteMessageBoxObject;
    public SortingGroup chatterSpriteGroup;
    public GameObject plinkoText;
    public GameObject crown;
    public GameObject prestigeBadge;
    public TextMeshProUGUI prestigeText;
    public GameObject magnifyingGlass;
    
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

    public static float spawnDepth = -2.0f;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private CabbageCharacter character;

    private void Awake()
    {
        this.chatterColor = this.GetRandomColor();

        this.hmmPhrases = new List<string>()
        {
            "HOW INTERESTING",
            "Let's get a closer look at that...",
            "AHA!!",
            "E N H A N C E",
            "Detective Mode: ACTIVATE!",
            "Stand aside, and leave this to a professional.",
            "Let's have a look...",
            "Oooohhhh, fascinating!",
            "I've never seen something like THIS before!",
            "Now what do we have here...",
            "Is that what I think it is?",
            "Oh my, what is THAT?"
        };
    }

    private void Start()
    {
        this.character = GetComponentInChildren<CabbageCharacter>();
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

    public void LoadCharacter(bool isPlinkoCabbage = false)
    {        
        if (isPlinkoCabbage == true)
        {
            this.plinkoText.GetComponent<TextMeshProUGUI>().text = this.chatterName;
            this.plinkoText.SetActive(true);
        }
        else
        {
            this.plinkoText.SetActive(false);
        }

        this.character.UpdateCharacter(this.chatterName);

        if (LeaderboardManager.instance.IsTopPlayer(this.chatterName))
        {
            CrownManager.UpdateCrownHolder(this.chatterName);
        }
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

    public void DisplayChatMessage(string chatterName, string chatMessage, bool isPlinkoCabbage = false)
    {
        this.chatterName = chatterName;

        chatCanvasObject.SetActive(true);

        if (isPlinkoCabbage == false)
        {
            this.LaunchAtRandomVelocity();
        }

        this.username.color = this.chatterColor;
        this.username.text = chatterName;

        

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

        this.LoadCharacter();
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

        this.gameObject.layer = LayerMask.NameToLayer("launchedCabbage");
    }

    private void DisableParticleSystem()
    {
        this.shootParticleObject.SetActive(false);
    }

    private IEnumerator ReduceShootCooldown()
    {
        yield return new WaitForSeconds(this.shootCooldown);        
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

            this.gameObject.layer = LayerMask.NameToLayer("launchedCabbage");
        }

        if (collision.gameObject.CompareTag("floor"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("cabbage");
        }
    }

    public void UpdateLayer(int layerDepth)
    {
        this.chatCanvas.sortingOrder = layerDepth;
        this.chatterSpriteGroup.sortingOrder = layerDepth;
    }

    private void LateUpdate()
    {
        if (this.chatCanvasObject.activeSelf == true && this.chatBoxObject.transform.childCount == 1)
        {
            this.chatCanvasObject.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            this.NukeCabbage();
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }

    //Prestige is disabled until I rethink how I want to store it on the database
    public void TriggerPrestige()
    {
        return;

        this.prestigeLevel++;
        this.shootScore -= CabbageManager.instance.prestigeThreshold;

        if (this.shootScore < 0)
        {
            this.shootScore = 0;
        }

        this.UpdatePrestigeBadge();

        float randomX = Random.Range(this.prestigeAnnouncementMinX, this.prestigeAnnouncementMaxX);
        float randomY = Random.Range(this.prestigeAnnouncementMinY, this.prestigeAnnouncementMaxY);

        Vector3 spawnPoint = new Vector3(randomX, randomY, CabbageChatter.spawnDepth);

        GameObject prestigeObject = Instantiate(this.prestigeAnnouncement, spawnPoint, new Quaternion());
        PrestigeAnimation prestigeComponent = prestigeObject.GetComponent<PrestigeAnimation>();
        prestigeComponent.SetCabbage(this);

        CabbageChatter.spawnDepth -= 0.01f;
    }

    public void ToggleMagnifyingGlass()
    {
        this.magnifyingGlass.SetActive(!this.magnifyingGlass.activeSelf);
    }

    public void NukeCabbage()
    {
        float xLaunchDirection = Random.Range(-0.5f, 0.5f);

        float yLaunchDirection = Random.Range(0.8f, 1.0f);
        this.cabbageRigidbody.AddForce(new Vector3(xLaunchDirection, yLaunchDirection, 0f) * this.nukeVelocity);
        this.shootParticleObject.SetActive(true);
        this.gameObject.layer = LayerMask.NameToLayer("NukedCabbage");

        StartCoroutine(this.ApplyArcForce());
    }

    private IEnumerator ApplyArcForce()
    {
        yield return new WaitForFixedUpdate();

        float directionalArcForce = (this.cabbageRigidbody.velocity.x > 0) ? -this.arcForce : this.arcForce;

        while (true)
        {
            this.cabbageRigidbody.AddForce(Quaternion.Euler(0, 0, 90) * this.cabbageRigidbody.velocity * directionalArcForce);
            yield return null;
        }
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
            this.KillCabbage();
        }
        else if (other.tag == "destroy")
        {
            CabbageManager.instance.RemoveCabbage(this.chatterName.ToLower());
        }
    }

    private void KillCabbage()
    {
        this.shootParticleObject.transform.parent = null;
        ParticleSystem.MainModule mainParticles = this.shootParticleObject.GetComponent<ParticleSystem>().main;
        mainParticles.loop = false;

        Vector3 orientationVector = -this.gameObject.transform.position;

        RaycastHit hit;

        int layerMask = LayerMask.GetMask("KillEffectCollider");

        Physics.Raycast(this.gameObject.transform.position, orientationVector, out hit, Mathf.Infinity, layerMask);

        Vector3 instantiationPoint = hit.point;

        GameObject deathEffectObject = Instantiate(this.deathEffect, instantiationPoint, new Quaternion()) as GameObject;

        deathEffectObject.transform.up = orientationVector;

        CabbageManager.instance.RemoveCabbage(this.chatterName);
    }
}
