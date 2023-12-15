using UnityEngine;

public class PlinkoLevel : MonoBehaviour
{
    [SerializeField]
    private DropZone[] dropZones;

    [HideInInspector]
    public PlinkoGame plinkoGame;

    // Start is called before the first frame update
    void Awake()
    {
        this.dropZones = GameObject.Find("DropZonesParent").GetComponentsInChildren<DropZone>();

        this.plinkoGame = GetComponentInParent<PlinkoGame>();
    }

    public bool IsValidDropIndex(int dropIndex)
    {
        for (int i = 0; i < this.dropZones.Length; i++)
        {
            if (dropIndex == this.dropZones[i].dropIndex)
            {
                return true;
            }
        }

        return false;
    }

    public void ProcessDropCommand(CabbageChatter cabbage, int dropNum)
    {
        for (int i = 0; i < this.dropZones.Length; i++)
        {
            if (dropNum == this.dropZones[i].dropIndex)
            {
                dropZones[i].DropCabbage(cabbage.gameObject);
            }
        }
    }
}
