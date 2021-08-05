using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrestigeAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer baseCabbage;
    [SerializeField]
    private SpriteRenderer headPiece;
    [SerializeField]
    private SpriteRenderer eyeBrows;
    [SerializeField]
    private SpriteRenderer eyes;
    [SerializeField]
    private SpriteRenderer nose;
    [SerializeField]
    private SpriteRenderer mouth;
    [SerializeField]
    private TextMeshProUGUI prestigeLevelText;

    public void SetCabbage(CabbageChatter chatter)
    {
        this.baseCabbage.sprite = chatter.baseCabbage.sprite;
        this.headPiece.sprite = chatter.headPiece.sprite;
        this.eyeBrows.sprite = chatter.eyeBrows.sprite;
        this.eyes.sprite = chatter.eyes.sprite;
        this.nose.sprite = chatter.nose.sprite;
        this.mouth.sprite = chatter.mouth.sprite;

        this.prestigeLevelText.text = chatter.prestigeLevel.ToString();
    }

    public void DestroyAnnouncement()
    {
        Destroy(this.gameObject);
    }
}
