public static class CrownManager
{    
    private static string currentCrownHolderUsername = "";

    public static void UpdateCrownHolder(string username)
    {
        if (CabbageManager.instance.DoesChatterExist(CrownManager.currentCrownHolderUsername))
        {
            CabbageManager.instance.GetCabbageChatter(CrownManager.currentCrownHolderUsername).DeactivateCrown();
        }

        if (CabbageManager.instance.DoesChatterExist(username))
        {
            CabbageManager.instance.GetCabbageChatter(username).ActivateCrown();
        }
        
        CrownManager.currentCrownHolderUsername = username;
    }
}
