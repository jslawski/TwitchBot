using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGame : ChatGame
{
    private FishSpawner fishSpawner;

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

    public override void Cleanup()
    {
        //Destroy all active fish
        this.fishSpawner.Cleanup();
    }
}
