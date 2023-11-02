using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDebugButton : MonoBehaviour
{
    private int clickCount = 5;

    private int currentConsecutiveClickCount = 0;

    private Coroutine timeoutCoroutine = null;

    [SerializeField]
    private GameObject debugCanvas;

    public void TrackClick()
    {
        this.currentConsecutiveClickCount++;

        if (this.timeoutCoroutine == null)
        {
            this.timeoutCoroutine = StartCoroutine(this.CheckForTimeout());
        }

        if (this.currentConsecutiveClickCount >= this.clickCount)
        {
            this.debugCanvas.SetActive(!this.debugCanvas.activeSelf);
            this.currentConsecutiveClickCount = 0;
            StopCoroutine(this.timeoutCoroutine);
            this.timeoutCoroutine = null;
        }
    }

    private IEnumerator CheckForTimeout()
    {
        float currentTimeElapsed = 0.0f;

        while (currentTimeElapsed < 3.0f)
        {
            currentTimeElapsed += Time.deltaTime;
            yield return null;
        }

        this.currentConsecutiveClickCount = 0;
        this.timeoutCoroutine = null;
    }
}
