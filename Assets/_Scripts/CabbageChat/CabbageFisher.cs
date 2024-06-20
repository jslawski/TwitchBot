using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbageFisher : MonoBehaviour
{    
    [SerializeField]
    private CabbageChatter chatter;

    private float moveSpeed = 3.0f;

    private float minViewportX = 0.1f;
    private float maxViewportX = 0.8f;

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

    public void StopMovement()
    {
        this.StopAllCoroutines();
    }
}
