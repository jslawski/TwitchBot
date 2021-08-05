using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    public static CharacterCreator instance;

    Sprite[] allHeadpieces;
    Sprite[] allEyebrows;
    Sprite[] allEyes;
    Sprite[] allNoses;
    Sprite[] allMouths;

    private void Awake()
    {
        CharacterCreator.instance = this;

        this.allHeadpieces = Resources.LoadAll<Sprite>("CharacterCreator/Headpiece");
        this.allEyebrows = Resources.LoadAll<Sprite>("CharacterCreator/Eyebrows");
        this.allEyes = Resources.LoadAll<Sprite>("CharacterCreator/Eyes");
        this.allNoses = Resources.LoadAll<Sprite>("CharacterCreator/Nose");
        this.allMouths = Resources.LoadAll<Sprite>("CharacterCreator/Mouth");
    }

    public Sprite GetHeadpiece()
    {
        return this.allHeadpieces[Random.Range(0, this.allHeadpieces.Length)];
    }

    public Sprite GetEyebrows()
    {
        return this.allEyebrows[Random.Range(0, this.allEyebrows.Length)];
    }

    public Sprite GetEyes()
    {
        return this.allEyes[Random.Range(0, this.allEyes.Length)];
    }

    public Sprite GetNose()
    {
        return this.allNoses[Random.Range(0, this.allNoses.Length)];
    }

    public Sprite GetMouth()
    {
        return this.allMouths[Random.Range(0, this.allMouths.Length)];
    }
}
