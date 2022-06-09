using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoLevel : MonoBehaviour
{
    private DropZone[] dropZones;

    [SerializeField]
    private GameObject cabbagePlinkoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        this.dropZones = GameObject.Find("DropZonesParent").GetComponentsInChildren<DropZone>();
    }

    public void ProcessDropCommand(CabbageChatter cabbage, int dropNum)
    {
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (dropNum == dropZones[i].dropIndex)
            {
                dropZones[i].DropCabbage(cabbage.gameObject);
            }
        }
    }
}
