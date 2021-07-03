using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    public void SetText(string newMessage)
    {
        messageText.text = newMessage;
    }

    public void AppendText(string additionalText)
    {
        messageText.text += " " + additionalText;
    }

    public void RemoveLastWord(int wordLength)
    {
        messageText.text = messageText.text.Remove(messageText.text.Length - wordLength);
    }
}
