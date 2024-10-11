using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    [SerializeField]
    private ChatGameManager chatGameManager;

    [SerializeField]
    private GameObject buzzerManagerObject;

    public void ProcessCommand(string username, string commandText, string arguments)
    {
        if (this.IsNinjaShotCommand(username, commandText))
        {
            GameObject.Find("ShotsRedemption").GetComponent<ShotsRedemption>().TriggerReward("Ninja");
        }

        if (commandText.Contains("hmm"))
        {
            this.ActivateHmmCommand(username);
        }
        
        if (username == "coleslawski")
        {
            if (commandText == "showclip")
            {
                this.ShowRecentClip();
            }
            else if (commandText == "buzz")
            {
                this.buzzerManagerObject.SetActive(!this.buzzerManagerObject.activeSelf);
            }
            else
            {
                this.chatGameManager.ProcessGameActivationCommand(commandText);
            }
            
        }

        if (this.chatGameManager.IsChatGameActive() == true)
        {
            this.chatGameManager.ProcessGameCommand(username, commandText, arguments);
        }        
    }

    private bool IsNinjaShotCommand(string username, string commandText)
    {
        return ((username == "safireninja" || username == "coleslawski") && commandText.ToLower().Contains("shot"));
    }

    private void ActivateHmmCommand(string username)
    {
        List<string> cabbageCodeVictors = new List<string> { "coleslawski", "ruddgasm", "ruddpuddle", "brainoidgames", "pomothedog", "spacey3d", "johngames", "roh_ka", "nickpea_and_thebean", "rookrules", "doctor_denny", "honestdangames" };

        if (cabbageCodeVictors.Contains(username))
        {
            if (CabbageManager.instance.DoesChatterExist(username))
            {
                CabbageManager.instance.GetCabbageChatter(username).ToggleMagnifyingGlass();
            }
            else
            {
                CabbageManager.instance.SpawnNewChatter(username);
                CabbageManager.instance.GetCabbageChatter(username).ToggleMagnifyingGlass();
            }
        }
        else
        {
            CabbageManager.instance.SendBotMessage("@" + username + " ACCESS DENIED! If you would access to this command, then you must solve the Cabbage Code at: https://jared-slawski.itch.io/the-cabbage-code");
        }
    }

    private void ShowRecentClip()
    {
        CabbageManager.instance.SendBotMessage("!loadclip " + CabbageManager.recentClip);
    }
}
