using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "ScriptableObjects/Fish")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite fishSprite;

    public int pointValue;

    public float minScale;
    public float maxScale;

    public float minDepth;
    public float maxDepth;

    public float minTimeBetweenMovement;
    public float maxTimeBetweenMovement;

    public float defaultHorizontalForce;

    public float defaultVerticalForce;

    public float GetRandomScale()
    {
        return Random.Range(this.minScale, this.maxScale);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return Vector3.zero;
    }

    public float GetNextMoveDelay()
    {
        return Random.Range(this.minTimeBetweenMovement, this.maxTimeBetweenMovement);
    }

    public Vector3 GetRandomMoveForce()
    {
        float verticalDirection = Random.Range(-1.0f, 1.0f);
        float moveDirection = Random.Range(-1.0f, 1.0f);

        Vector3 verticalVector = Vector3.up * verticalDirection * this.defaultVerticalForce;
        Vector3 horizontalVector = Vector3.right * moveDirection * this.defaultHorizontalForce;

        return (verticalVector + horizontalVector);
    }
}
