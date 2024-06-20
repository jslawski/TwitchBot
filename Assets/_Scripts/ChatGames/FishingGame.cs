using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGame : ChatGame
{
    private FishSpawner fishSpawner;

    private float secondsBetweenFishSpawns = 20f;

    [SerializeField]
    private GameObject waterObject;
    [SerializeField]
    private AnimationCurve waterRevealCurve;
    private Vector3 waterStartPosition;
    private Vector3 waterEndPosition;

    [SerializeField]
    private GameObject leftWall;
    [SerializeField]
    private GameObject rightWall;

    public override void Setup()
    {
        this.leftWall.SetActive(false);
        this.rightWall.SetActive(false);

        this.SetupCabbages();

        this.SetupWater();

        //Spawn initial fish
        this.fishSpawner = GetComponentInChildren<FishSpawner>();

        //Start fish spawning coroutine
        StartCoroutine(this.FishSpawningCoroutine());
    }

    public override void ProcessCommand(string username, string commandText, string argumentsAsString = "")
    {
        if (CabbageManager.instance.DoesChatterExist(username) == false)
        {
            //Do a spawn of some sort
        }

        if (commandText.Contains("left"))
        {
            CabbageManager.instance.GetCabbageChatter(username).GetComponentInChildren<CabbageFisher>().MoveLeft();
        }
        else if (commandText.Contains("right"))
        {
            CabbageManager.instance.GetCabbageChatter(username).GetComponentInChildren<CabbageFisher>().MoveRight();
        }
    }

    public override IEnumerator AICoroutine()
    {
        yield return null;
    }

    public override void Cleanup()
    {
        this.StopAllCoroutines();
        
        this.fishSpawner.Cleanup();
        this.CleanupCabbages();
        this.CleanupWater();

        this.leftWall.SetActive(true);
        this.rightWall.SetActive(true);
    }

    private void SetupCabbages()
    {
        CabbageChatter[] activeChatters = CabbageManager.instance.parentChat.GetComponentsInChildren<CabbageChatter>();

        for (int i = 0; i < activeChatters.Length; i++)
        {
            SpinCabbage cabbageSpinner = activeChatters[i].GetComponentInChildren<SpinCabbage>();
            cabbageSpinner.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            cabbageSpinner.enabled = false;

            activeChatters[i].SuspendCabbage();
        }
    }

    private void CleanupCabbages()
    {
        CabbageChatter[] activeChatters = CabbageManager.instance.parentChat.GetComponentsInChildren<CabbageChatter>();

        for (int i = 0; i < activeChatters.Length; i++)
        {
            SpinCabbage cabbageSpinner = activeChatters[i].GetComponentInChildren<SpinCabbage>();
            cabbageSpinner.enabled = true;

            activeChatters[i].ToggleBoatAndRod();
            activeChatters[i].character.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void SetupWater()
    {
        this.waterStartPosition = new Vector3(-4.0f, -40.0f, 0.0f);
        this.waterEndPosition = new Vector3(-4.0f, -3.0f, 0.0f);

        this.waterObject.SetActive(true);

        StartCoroutine(this.RevealWater());
    }

    private void CleanupWater()
    {
        StartCoroutine(this.HideWater());
    }

    private IEnumerator RevealWater()
    {
        float currentT = 0.0f;
        while (currentT < 1.0f)
        {
            float animationValue = this.waterRevealCurve.Evaluate(currentT);

            this.waterObject.transform.position = Vector3.Lerp(this.waterStartPosition, this.waterEndPosition, animationValue);

            currentT += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        CabbageChatter[] activeChatters = CabbageManager.instance.parentChat.GetComponentsInChildren<CabbageChatter>();

        for (int i = 0; i < activeChatters.Length; i++)
        {
            activeChatters[i].ToggleBoatAndRod();
            activeChatters[i].UnsuspendCabbage();
        }

        yield return new WaitForSeconds(1.0f);

        this.fishSpawner.SpawnInitialFishes();
    }

    private IEnumerator HideWater()
    {
        float currentT = 1.0f;
        while (currentT > 0.0f)
        {
            float animationValue = this.waterRevealCurve.Evaluate(currentT);

            this.waterObject.transform.position = Vector3.Lerp(this.waterStartPosition, this.waterEndPosition, animationValue);

            currentT -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        this.waterObject.SetActive(false);
    }

    private IEnumerator FishSpawningCoroutine()
    {
        yield return null;
    
        while (this.gameActive == true)
        {
            yield return new WaitForSeconds(this.secondsBetweenFishSpawns);

            Fish[] currentFish = this.fishSpawner.gameObject.GetComponentsInChildren<Fish>();

            if (currentFish.Length < this.fishSpawner.minFish)
            {
                this.fishSpawner.SpawnFishGroup(this.fishSpawner.minFish - currentFish.Length);
            }
            else if (currentFish.Length < this.fishSpawner.maxFish)
            {
                this.fishSpawner.SpawnSingleFish();
            }
        }
    }
}
