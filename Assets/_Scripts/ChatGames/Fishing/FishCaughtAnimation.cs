using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCustomizer;
using TMPro;
using UnityEngine.UI;

public class FishCaughtAnimation : MonoBehaviour
{
    [SerializeField]
    private CabbageCharacter character;
    [SerializeField]
    private TextMeshProUGUI usernameText;
    [SerializeField]
    private TextMeshProUGUI fishText;
    [SerializeField]
    private TextMeshProUGUI weightText;
    [SerializeField]
    private TextMeshProUGUI pointsText;
    [SerializeField]
    private Image fishImage;

    private float minViewportX = 0.3f;
    private float maxViewportX = 0.6f;
    private float minViewportY  = 0.3f;
    private float maxViewportY = 0.6f;

    public void Setup(CabbageChatter chatter, FishData fish, float size)
    {
        float randomX = Random.Range(this.minViewportX, this.maxViewportX);
        float randomY = Random.Range(this.minViewportY, this.maxViewportY);

        Vector3 viewportVector = new Vector3(randomX, randomY, -Camera.main.transform.position.z);

        this.gameObject.transform.position = Camera.main.ViewportToWorldPoint(viewportVector);

        this.character.UpdateCharacter(chatter.chatterName);

        this.fishImage.sprite = fish.fishSprite;

        this.usernameText.text = chatter.chatterName + " caught:";
        this.fishText.text = fish.fishName;

        this.weightText.text = System.Math.Round(size, 2).ToString() + " lbs";
        this.pointsText.text = fish.pointValue.ToString() + " pts";
    }

    public void DestroyAnnouncement()
    {
        Destroy(this.gameObject);
    }
}
