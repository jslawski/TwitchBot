using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallGame : ChatGame
{
    private GameObject[] bballLevels;
    private GameObject activeBBallLevel;

    public override void ToggleGame()
    {
        if (this.gameActive == false)
        {
            this.Setup();
        }
        else
        {
            this.Cleanup();
        }
    }

    public override void Setup()
    {
        this.bballLevels = Resources.LoadAll<GameObject>("BBallLevels");

        this.RandomlyPickLevel();

        StartCoroutine(this.AICoroutine());

        this.gameActive = true;
    }

    public override void ProcessCommand(string username, string commandText, string argumentsAsString = "")
    {
        if (commandText.Contains("shoot"))
        {
            if (CabbageManager.instance.DoesChatterExist(username) == false)
            {
                CabbageManager.instance.SpawnNewChatter(username);
            }

            CabbageManager.instance.GetCabbageChatter(username).ShootCharacter(argumentsAsString);
        }
    }

    public override IEnumerator AICoroutine()
    {
        yield return new WaitForSeconds(this.secondsBetweenAIAction);

        while (this.activeBBallLevel != null)
        {
            if (CabbageManager.instance.DoesChatterExist("cabbagegatekeeper"))
            {
                CabbageManager.instance.GetCabbageChatter("cabbagegatekeeper").ShootCharacter();
            }
            else
            {
                CabbageManager.instance.SendBotMessage("!shoot");
            }

            yield return new WaitForSeconds(this.secondsBetweenAIAction);
        }
    }

    public override void Cleanup()
    {
        StopAllCoroutines();

        Destroy(this.activeBBallLevel);

        this.gameActive = false;
    }

    private void RandomlyPickLevel()
    {
        int levelIndex = Random.Range(0, this.bballLevels.Length);
        GameObject randomLevel = this.bballLevels[levelIndex];

        this.activeBBallLevel = Instantiate(randomLevel, this.transform);
    }
}
