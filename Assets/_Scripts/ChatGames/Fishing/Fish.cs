using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private FishData fishData;

    private bool hooked = false;

    private Rigidbody rigidbody;
    private SpriteRenderer spriteRenderer;

    public void Setup(FishData fishType)
    {
        this.fishData = fishType;

        this.transform.position = this.fishData.GetRandomSpawnPoint();

        float randomScale = this.fishData.GetRandomScale();
        this.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody>();
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(this.FishAI());
    }

    private void FixedUpdate()
    {
        //this.UpdateSpriteDirection();
    }

    private IEnumerator FishAI()
    {
        while (this.hooked == false)
        {
            yield return new WaitForSeconds(this.fishData.GetNextMoveDelay());

            Vector3 randomForce = this.fishData.GetRandomMoveForce();
            this.rigidbody.AddForce(this.fishData.GetRandomMoveForce(), ForceMode.Impulse);

            yield return null;

            this.UpdateSpriteDirection();
        }
    }

    private void UpdateSpriteDirection()
    {
        if (this.rigidbody.velocity.x >= 0.0f)
        {
            this.spriteRenderer.flipX = false;
        }
        else
        {
            this.spriteRenderer.flipX = true;
        }
    }
}
