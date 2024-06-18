using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGame : ChatGame
{
    [SerializeField]
    private GameObject fishPrefab;

    [SerializeField]
    private GameObject fishParent;

    public FishData testFish;

    public override void Setup()
    {
        //Disable floor, let cabbages drop down.  Wait for them all to be gone
        //Spawn active chat members at the top of the water?

        //Enable left and right boundary walls
        //Display water

        //Spawn initial fish
        this.SpawnInitialFish();

        //Start fish spawning coroutine

        //Start AI Coroutine
    }

    private void SpawnInitialFish()
    {
        GameObject spawnedFish = Instantiate(this.fishPrefab, this.fishParent.transform);
        Fish fishComponent = spawnedFish.GetComponent<Fish>();
        fishComponent.Setup(testFish);
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
    }
}
