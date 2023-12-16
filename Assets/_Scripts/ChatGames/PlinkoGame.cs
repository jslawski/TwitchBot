using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoGame : ChatGame
{
    [SerializeField]
    private GameObject cabbageChatterPrefab;
    [SerializeField]
    private GameObject plinkoUsersParentObject;
    [SerializeField]
    private GameObject floorCollider;
    [SerializeField]
    private GameObject leftWallCollider;
    [SerializeField]
    private GameObject destroyCollider;

    private List<int> seenPlinkoLevels;
    private List<int> unseenPlinkoLevels;

    private GameObject activePlinkoLevel;
    
    private GameObject[] allPlinkoLevels;

    public override void Setup()
    {
        this.SetupPlinkoColliders();

        this.allPlinkoLevels = Resources.LoadAll<GameObject>("PlinkoLevels");

        this.SetupSeenAndUnseenLevelLists();

        StartCoroutine(this.ActivatePlinkoLevel());
        StartCoroutine(this.AICoroutine());
    }

    public override void ProcessCommand(string username, string commandText, string argumentsAsString = "")
    {
        int plinkoDropZone = 0;

        if (commandText.Contains("rand"))
        {
            GameObject dropZonesParentObject = GameObject.Find("DropZonesParent");

            if (dropZonesParentObject != null)
            {
                this.AttemptPlinkoDrop(username, Random.Range(1, dropZonesParentObject.transform.childCount + 1));
            }
        }
        else if (int.TryParse(commandText, out plinkoDropZone))
        {
            this.AttemptPlinkoDrop(username, plinkoDropZone);
        }
    }

    public override IEnumerator AICoroutine()
    {
        yield return new WaitForSeconds(this.secondsBetweenAIAction);

        while (this.plinkoUsersParentObject.activeSelf == true)
        {
            GameObject dropZonesParentObject = GameObject.Find("DropZonesParent");

            if (dropZonesParentObject != null)
            {
                this.AttemptPlinkoDrop("cabbagegatekeeper",
                    Random.Range(1, dropZonesParentObject.transform.childCount + 1));
            }

            yield return new WaitForSeconds(this.secondsBetweenAIAction);
        }
    }

    public override void Cleanup()
    {
        StartCoroutine(this.CleanupPlinko());
    }

    private IEnumerator CleanupPlinko()
    {
        Destroy(this.activePlinkoLevel.gameObject);

        while (CabbageManager.instance.GetCurrentActiveChattersCount() > 0)
        {
            yield return null;
        }

        this.SetupDefaultChatColliders();

        StopAllCoroutines();
    }

    private void SetupPlinkoColliders()
    {
        this.floorCollider.SetActive(false);
        this.leftWallCollider.SetActive(false);
        this.destroyCollider.SetActive(true);
    }

    private void SetupDefaultChatColliders()
    {
        this.floorCollider.SetActive(true);
        this.leftWallCollider.SetActive(true);
        this.destroyCollider.SetActive(false);
    }

    private void SetupSeenAndUnseenLevelLists()
    {
        this.seenPlinkoLevels = new List<int>();
        this.unseenPlinkoLevels = new List<int>();
        for (int i = 0; i < this.allPlinkoLevels.Length; i++)
        {
            this.unseenPlinkoLevels.Add(i);
        }
    }

    public void AttemptPlinkoDrop(string username, int dropIndex)
    {
        PlinkoLevel currentPlinkoLevel = this.activePlinkoLevel.GetComponent<PlinkoLevel>();

        if (currentPlinkoLevel == null || CabbageManager.instance.DoesChatterExist(username) == true || !currentPlinkoLevel.IsValidDropIndex(dropIndex) == true)
        {
            return;
        }

        CabbageChatter droppedCabbage = CabbageManager.instance.AddCabbage(username);
        droppedCabbage.transform.parent = this.plinkoUsersParentObject.transform;

        droppedCabbage.chatterName = username;
        droppedCabbage.gameObject.name = username;

        droppedCabbage.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        currentPlinkoLevel.ProcessDropCommand(droppedCabbage, dropIndex);

        droppedCabbage.LoadCharacter(true);
    }

    public void LoadNewPlinkoLevel()
    {
        StartCoroutine(ActivatePlinkoLevel());
    }

    private int GetNewPlinkoLevelIndex()
    {
        int randomPlinkoLevelIndex = this.unseenPlinkoLevels[UnityEngine.Random.Range(0, this.unseenPlinkoLevels.Count)];

        this.seenPlinkoLevels.Add(randomPlinkoLevelIndex);
        this.unseenPlinkoLevels.Remove(randomPlinkoLevelIndex);

        if (this.unseenPlinkoLevels.Count == 0)
        {
            this.unseenPlinkoLevels = this.seenPlinkoLevels;
            this.seenPlinkoLevels = new List<int>();
        }

        return randomPlinkoLevelIndex;
    }

    private IEnumerator ActivatePlinkoLevel()
    {
        if (this.activePlinkoLevel != null)
        {
            Destroy(this.activePlinkoLevel.gameObject);
            yield return null;
        }

        while (CabbageManager.instance.GetCurrentActiveChattersCount() > 0)
        {
            yield return null;
        }

        GameObject randomLevel = this.allPlinkoLevels[this.GetNewPlinkoLevelIndex()];

        this.activePlinkoLevel = Instantiate(randomLevel, this.transform);
    }
}
