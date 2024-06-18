using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject fishPrefab;
    
    private int minFish = 10;
    private int maxFish = 10;

    private float uncommonFishSpawnChance = 0.40f;
    private float rareFishSpawnChance = 0.20f;
    private float superRareFishSpawnChance = 0.05f;

    public void SpawnInitialFishes()
    {
        for (int i = 0; i < this.maxFish; i++)
        {
            this.SpawnSingleFish();
        }
    }

    public void SpawnSingleFish()
    {
        Fish[] currentFish = GetComponentsInChildren<Fish>();

        if (currentFish.Length >= this.maxFish)
        {
            return;
        }

        FishData randomFishData = this.GetRandomFishData();

        GameObject spawnedFish = Instantiate(this.fishPrefab, this.gameObject.transform);
        Fish fishComponent = spawnedFish.GetComponent<Fish>();
        fishComponent.Setup(randomFishData);
    }

    private FishData GetRandomFishData()
    {
        float randomRoll = Random.Range(0.0f, 1.0f);

        FishData[] potentialFish;

        if (randomRoll <= this.superRareFishSpawnChance)
        {
            potentialFish = Resources.LoadAll<FishData>("FishData/SuperRare");
        }
        else if (randomRoll <= this.rareFishSpawnChance)
        {
            potentialFish = Resources.LoadAll<FishData>("FishData/Rare");
        }

        else if (randomRoll <= this.uncommonFishSpawnChance)
        {
            potentialFish = Resources.LoadAll<FishData>("FishData/Uncommon");
        }
        else
        {
            potentialFish = Resources.LoadAll<FishData>("FishData/Common");
        }

        int randomIndex = Random.Range(0, potentialFish.Length);

        return potentialFish[randomIndex];
    }

    public void Cleanup()
    {
        Fish[] currentFish = GetComponentsInChildren<Fish>();

        for (int i = 0; i < currentFish.Length; i++)
        {
            currentFish[i].Cleanup();
        }
    }
}
