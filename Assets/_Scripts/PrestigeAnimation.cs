using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CharacterCustomizer;


//TODO RETHINK PRESTIGE ENTIRELY
public class PrestigeAnimation : MonoBehaviour
{
    [SerializeField]
    private CustomCharacter character;    
    private TextMeshProUGUI prestigeLevelText;

    public void SetCabbage(CabbageChatter chatter)
    {
        this.prestigeLevelText.text = chatter.prestigeLevel.ToString();
    }

    public void SetCabbage(SpriteRenderer baseCabbage, SpriteRenderer headPiece, 
        SpriteRenderer eyeBrows, SpriteRenderer eyes, SpriteRenderer nose, 
        SpriteRenderer mouth, int prestigeLevel)
    {

        this.prestigeLevelText.text = prestigeLevel.ToString();
    }

    public void DestroyAnnouncement()
    {
        Destroy(this.gameObject);
    }
}
