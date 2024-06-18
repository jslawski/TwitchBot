using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private FishData fishData;

    private bool hooked = false;

    private Rigidbody fishRigidbody;
    private SpriteRenderer spriteRenderer;

    public void Setup(FishData fishType)
    {
        this.fishData = fishType;

        this.spriteRenderer.sprite = this.fishData.fishSprite;

        this.transform.position = this.fishData.GetRandomSpawnPoint();

        float randomScale = this.fishData.GetRandomScale();
        this.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void Awake()
    {
        this.fishRigidbody = GetComponent<Rigidbody>();
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(this.FishAI());
    }

    private void FixedUpdate()
    {
        this.UpdateSpriteDirection();
    }

    private IEnumerator FishAI()
    {
        while (this.hooked == false)
        {
            yield return new WaitForSeconds(this.fishData.GetNextMoveDelay());

            this.fishRigidbody.velocity = Vector3.zero;

            Vector3 randomForce = this.fishData.GetRandomMoveForce();
            this.fishRigidbody.AddForce(this.fishData.GetRandomMoveForce(), ForceMode.Impulse);
        }
    }

    private void UpdateSpriteDirection()
    {
        if (this.fishRigidbody.velocity.x >= 0.0f)
        {
            this.spriteRenderer.flipX = false;
        }
        else
        {
            this.spriteRenderer.flipX = true;
        }
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
