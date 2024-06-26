using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallGame : ChatGame
{
    private GameObject[] bballLevels;
    private GameObject activeBBallLevel;

    public override void Setup()
    {
        this.bballLevels = Resources.LoadAll<GameObject>("BBallLevels");

        this.RandomlyPickLevel();

        StartCoroutine(this.AICoroutine());
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

            //Chaos
            if (CabbageManager.instance.TestChatterExists() == true)
            {
                for (int i = 0; i < CabbageManager.instance.currentActiveChatters.Count; i++)
                {
                    if (CabbageManager.instance.currentActiveChatters[i].chatterName.Contains("testcabbage") == true)
                    {
                        CabbageManager.instance.GetCabbageChatter(CabbageManager.instance.currentActiveChatters[i].chatterName).ShootCharacter();
                    }
                }
            }

            yield return new WaitForSeconds(this.secondsBetweenAIAction);
        }
    }

    public override void Cleanup()
    {
        StopAllCoroutines();

        Destroy(this.activeBBallLevel);
    }

    private void RandomlyPickLevel()
    {
        int levelIndex = Random.Range(0, this.bballLevels.Length);
        GameObject randomLevel = this.bballLevels[levelIndex];

        this.activeBBallLevel = Instantiate(randomLevel, this.transform);
    }
}
