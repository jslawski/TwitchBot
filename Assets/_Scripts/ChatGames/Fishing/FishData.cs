using UnityEngine;

public enum FishRarity { Common, Uncommon, Rare, SuperRare };

[CreateAssetMenu(fileName = "NewFish", menuName = "ScriptableObjects/Fish")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite fishSprite;

    public FishRarity rarity;

    public int pointValue;

    public float minScale;
    public float maxScale;

    public float minViewportHeight;
    public float maxViewportHeight;

    public float minTimeBetweenMovement;
    public float maxTimeBetweenMovement;

    public float minHorizontalForce;
    public float maxHorizontalForce;

    public float minVerticalForce;
    public float maxVerticalForce;

    public float GetRandomScale()
    {
        return Random.Range(this.minScale, this.maxScale);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        float randomViewportX = Random.Range(0.1f, 0.7f);
        float randomViewportY = Random.Range(this.minViewportHeight, this.maxViewportHeight);

        Vector3 viewportVector = new Vector3(randomViewportX, randomViewportY, -Camera.main.transform.position.z);

        return (Camera.main.ViewportToWorldPoint(viewportVector));
    }

    public float GetNextMoveDelay()
    {
        return Random.Range(this.minTimeBetweenMovement, this.maxTimeBetweenMovement);
    }

    public Vector3 GetRandomMoveForce()
    {
        int horizontalDirection = Random.Range(0, 2);
        int verticalDirection = Random.Range(0, 2);

        float horizontalMagnitude = Random.Range(this.minHorizontalForce, this.maxHorizontalForce);
        float verticalMagnitude = Random.Range(this.minVerticalForce, this.maxVerticalForce);

        Vector3 horizontalVector = Vector3.right * horizontalMagnitude;
        Vector3 verticalVector = Vector3.up * verticalMagnitude;

        if (horizontalDirection == 0)
        {
            horizontalVector *= -1;
        }

        if (verticalDirection == 0)
        {
            verticalVector *= -1;
        }

        return (horizontalVector + verticalVector);
    }
}
