using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHook : MonoBehaviour
{
    [SerializeField]    
    private Transform hookOrigin;

    [SerializeField]
    private LineRenderer fishingLine;

    private float defaultReelSpeed = 5.0f;
    private float minViewportY = 0.01f;

    public Fish hookedFish;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.UpdateFishingLine();

        if (this.hookedFish != null)
        {
            this.hookedFish.gameObject.transform.position = this.gameObject.transform.position;
        }
    }

    private void UpdateFishingLine()
    {
        this.fishingLine.SetPosition(0, this.hookOrigin.position);
        this.fishingLine.SetPosition(1, this.gameObject.transform.position);
    }

    public void Cast()
    {
        this.StopAllCoroutines();
        StartCoroutine(this.CastCoroutine());
    }

    private IEnumerator CastCoroutine()
    {
        Vector3 viewportVector = new Vector3(0.0f, this.minViewportY, -Camera.main.transform.position.z);
        float oceanFloorY = Camera.main.ViewportToWorldPoint(viewportVector).y;

        while (this.gameObject.transform.position.y >= oceanFloorY)
        {
            this.gameObject.transform.Translate(Vector3.down * this.defaultReelSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Hang()
    {
        this.StopAllCoroutines();
    }

    public void Reel()
    {
        this.StopAllCoroutines();
        StartCoroutine(this.ReelCoroutine(this.defaultReelSpeed));
    }

    private IEnumerator ReelCoroutine(float reelSpeed)
    {
        while (this.gameObject.transform.position.y <= this.hookOrigin.position.y)
        {
            this.gameObject.transform.Translate(Vector3.up * reelSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        if (this.hookedFish != null)
        {
            this.Catch();
        }
    }

    private void ReelHookedFish()
    {
        float reelSpeed = this.defaultReelSpeed;// / this.hookedFish.gameObject.transform.localScale.x;
        StartCoroutine(this.ReelCoroutine(reelSpeed));

        GetComponentInParent<CabbageFisher>().StopMovement();
    }

    public void InitiateHook(Fish hookedFish)
    {
        this.hookedFish = hookedFish;
        this.StopAllCoroutines();

        this.ReelHookedFish();
    }

    private void Catch()
    {
        GetComponentInParent<CabbageFisher>().CatchFish(this.hookedFish.fishData, this.hookedFish.gameObject.transform.localScale.x);
        
        Destroy(this.hookedFish.gameObject);
        this.hookedFish = null;
    }
}
