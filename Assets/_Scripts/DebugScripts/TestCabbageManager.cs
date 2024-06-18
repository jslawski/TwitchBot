public static class TestCabbageManager
{
    public static int nextTestCabbageID = 0;
    
    public static int GetNewTestCabbageID()
    {
        return nextTestCabbageID++;
    }

    public static void ClearTestCabbages()
    {
        for (int i = 0; i < CabbageManager.instance.currentActiveChatters.Count; i++)
        {
            if (CabbageManager.instance.currentActiveChatters[i].chatterName.Contains("testcabbage") == true)
            {
                CabbageManager.instance.RemoveCabbage(CabbageManager.instance.currentActiveChatters[i].chatterName);
            }
        }

        TestCabbageManager.nextTestCabbageID = 0;
    }    
}
