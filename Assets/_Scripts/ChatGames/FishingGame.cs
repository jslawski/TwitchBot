using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGame : ChatGame
{
    private FishSpawner fishSpawner;

    private float secondsBetweenFishSpawns = 20f;

    public override void Setup()
    {
        this.fishSpawner = GetComponentInChildren<FishSpawner>();

        //Disable floor, let cabbages drop down.  Wait for them all to be gone
        //Spawn active chat members at the top of the water?

        //Enable left and right boundary walls
        //Display water

        //Spawn initial fish
        this.fishSpawner.SpawnInitialFishes();

        //Start fish spawning coroutine
        StartCoroutine(this.FishSpawningCoroutine());
        //Start AI Coroutine
    }
    
    public override void ProcessCommand(string username, string commandText, string argumentsAsString = "")
    {
        //!left, !right, !stop, !cast
    }

    public override IEnumerator AICoroutine()
    {
        yield return null;
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

    public override void Cleanup()
    {
        this.StopAllCoroutines();
    
        //Destroy all active fish
        this.fishSpawner.Cleanup();
    }
}
