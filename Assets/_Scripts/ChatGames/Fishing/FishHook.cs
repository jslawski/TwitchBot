using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHook : MonoBehaviour
{
    [SerializeField]    
    private Transform hookOrigin;

    [SerializeField]
    private LineRenderer fishingLine;

    private float moveSpeed = 3.0f;
    private float minViewportY = 0.01f;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.UpdateFishingLine();
    }

    private void UpdateFishingLine()
    {
        this.fishingLine.SetPosition(0, this.hookOrigin.position);
        this.fishingLine.SetPosition(1, this.gameObject.transform.position);
    }

    public void Cast()
    {
        StartCoroutine(this.CastCoroutine());
    }

    private IEnumerator CastCoroutine()
    {
        Vector3 viewportVector = new Vector3(0.0f, this.minViewportY, -Camera.main.transform.position.z);
        float oceanFloorY = Camera.main.ViewportToWorldPoint(viewportVector).y;

        while (this.gameObject.transform.position.y >= oceanFloorY)
        {
            this.gameObject.transform.Translate(Vector3.down * this.moveSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Hang()
    {
        this.StopAllCoroutines();
    }

    public void Reel()
    {
        StartCoroutine(this.ReelCoroutine());
    }

    private IEnumerator ReelCoroutine()
    {
        while (this.gameObject.transform.position.y <= this.hookOrigin.position.y)
        {
            this.gameObject.transform.Translate(Vector3.up * this.moveSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
