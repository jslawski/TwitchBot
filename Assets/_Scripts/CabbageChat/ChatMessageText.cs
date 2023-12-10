using System.Collections;
using TMPro;
using UnityEngine;

public class ChatMessageText : MonoBehaviour
{
    public TextMeshProUGUI chatMessage;
    private float messageUptime = 15f;

    public void DisplayMessage(string messageText)
    {
        this.chatMessage.text = messageText;

        StartCoroutine(this.DestroyMessageAfterDelay());
    }

    private IEnumerator DestroyMessageAfterDelay()
    {
        yield return new WaitForSeconds(this.messageUptime);
        Destroy(this.gameObject);
    }

    public void DestroyEarly()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

}
