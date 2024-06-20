using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbageFisher : MonoBehaviour
{    
    [SerializeField]
    private CabbageChatter chatter;

    private FishHook hook;

    private float moveSpeed = 3.0f;

    private float minViewportX = 0.1f;
    private float maxViewportX = 0.8f;

    public void Setup()
    {
        this.hook = GetComponentInChildren<FishHook>();
    
        this.chatter.gameObject.transform.localScale = Vector3.one * 0.5f;
    
        this.chatter.EnableBoatAndRod();

        this.chatter.cabbageRigidbody.velocity = Vector3.zero;

        SpinCabbage cabbageSpinner = chatter.GetComponentInChildren<SpinCabbage>();
        cabbageSpinner.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        cabbageSpinner.enabled = false;
    }

    public void Cleanup()
    {
        this.chatter.gameObject.transform.localScale = Vector3.one * 0.8f;

        this.chatter.DisableBoatAndRod();

        SpinCabbage cabbageSpinner = chatter.GetComponentInChildren<SpinCabbage>();
        cabbageSpinner.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        cabbageSpinner.enabled = true;

        this.chatter.character.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void MoveLeft()
    {
        this.StopMovement();
        this.chatter.character.gameObject.transform.localRotation = Quaternion.Euler(0.0f, -180.0f, 0.0f);
        StartCoroutine(this.MoveLeftCoroutine());
    }

    private IEnumerator MoveLeftCoroutine()
    {
        Vector3 viewportVector = new Vector3(minViewportX, 0.0f, -Camera.main.transform.position.z);
        float leftWallWorldX = Camera.main.ViewportToWorldPoint(viewportVector).x;
        
        while (this.chatter.gameObject.transform.position.x >= leftWallWorldX)
        {
            this.chatter.gameObject.transform.Translate(Vector3.left * this.moveSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    public void MoveRight()
    {
        this.StopMovement();
        this.chatter.character.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 0, 0.0f);
        StartCoroutine(this.MoveRightCoroutine());
    }

    private IEnumerator MoveRightCoroutine()
    {
        Vector3 viewportVector = new Vector3(maxViewportX, 0.0f, -Camera.main.transform.position.z);
        float rightWallWorldX = Camera.main.ViewportToWorldPoint(viewportVector).x;

        while (this.chatter.gameObject.transform.position.x <= rightWallWorldX)
        {
            this.chatter.gameObject.transform.Translate(Vector3.right * this.moveSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Cast()
    {
        this.hook.Cast();
    }

    public void Reel()
    {
        this.hook.Reel();
    }

    public void Hang()
    {
        this.hook.Hang();
        this.StopMovement();
    }

    public void StopMovement()
    {
        this.StopAllCoroutines();
    }
}
