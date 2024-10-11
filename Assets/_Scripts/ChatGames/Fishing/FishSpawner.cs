using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject fishPrefab;
    
    public int minFish = 10;
    public int maxFish = 20;

    private float uncommonFishSpawnChance = 0.30f;
    private float rareFishSpawnChance = 0.15f;
    private float superRareFishSpawnChance = 0.05f;

    public void SpawnInitialFishes()
    {
        int randomSpawnNumber = Random.Range(this.minFish, this.maxFish);
    
        for (int i = 0; i < randomSpawnNumber; i++)
        {
            this.SpawnSingleFish();
        }
    }

    public void SpawnFishGroup(int fishToSpawn)
    {
        for (int i = 0; i < fishToSpawn; i++)
        {
            this.SpawnSingleFish();
        }
    }

    public void SpawnSingleFish()
    {
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
