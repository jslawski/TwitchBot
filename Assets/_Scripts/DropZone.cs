using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropZone : MonoBehaviour
{
    public int dropIndex = 0;

    [SerializeField]
    private BoxCollider collider;

    [SerializeField]
    private TextMeshProUGUI commandText;

    private void Awake()
    {
        this.commandText.text = "!" + dropIndex.ToString();
    }

    public void DropCabbage(GameObject newCabbage)
    {
        float minX = this.collider.transform.position.x - (this.collider.bounds.size.x / 2.0f);
        float maxX = this.collider.transform.position.x + (this.collider.bounds.size.x / 2.0f);
        float minY = this.collider.transform.position.y - (this.collider.bounds.size.y / 2.0f);
        float maxY = this.collider.transform.position.y + (this.collider.bounds.size.y / 2.0f);

        float randX = Random.Range(minX, maxX);
        float randY = Random.Range(minY, maxY);

        newCabbage.transform.position = new Vector3(randX, randY, -0.1f);
    }
}
