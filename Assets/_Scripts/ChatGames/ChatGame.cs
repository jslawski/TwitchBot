using System.Collections;
using UnityEngine;

public abstract class ChatGame : MonoBehaviour
{
    public float secondsBetweenAIAction = 45.0f;

    public bool gameActive = false;

    public abstract void ToggleGame();

    public abstract void Setup();
    
    public abstract void ProcessCommand(string username, string commandText, string argumentsAsString = "");

    public abstract IEnumerator AICoroutine();

    public abstract void Cleanup();
}
