using UnityEngine;
using CharacterCustomizer;

public class CabbageCharacter : MonoBehaviour
{
    public CustomCharacter character;

    private string username;

    public void UpdateCharacter(string username, bool forceRefresh = false)
    {
        this.username = username;

        //First check cache, then ping backend if needed
        if (forceRefresh == false && CharacterCache.IsCached(this.username))
        {
            this.character.LoadCharacterFromJSON(CharacterCache.GetCachedSettings(this.username));
            this.ScaleCharacter();
        }
        else
        {
            GetCurrentPresetAsyncRequest request = new GetCurrentPresetAsyncRequest(this.username, this.GetCurrentPresetSuccess, this.GetCurrentPresetFailure);
            request.Send();
        }
    }

    private void GetCurrentPresetSuccess(string data)
    {
        this.character.LoadCharacterFromJSON(data);

        //Cache Attribute Settings
        CharacterCache.UpdateCache(this.username, data);

        this.ScaleCharacter();
    }

    private void GetCurrentPresetFailure()
    {
        Debug.LogError("Error: Unable to fetch user's current preset");
    }

    private void ScaleCharacter()
    {
        CharacterAttribute baseAttribute = this.character.GetAttribute(AttributeType.BaseCabbage);

        float xDiff = (baseAttribute.GetScaleX() - 1.0f) / 2.0f;
        float yDiff = (baseAttribute.GetScaleY() - 1.0f) / 2.0f;

        //Scale up
        if (xDiff <= 0 && yDiff <= 0)
        {
            return;
        }

        float newDiff = 0.0f;

        //Scale based on the biggest diff
        if (xDiff > yDiff)
        {
            newDiff = 1.0f - xDiff;
        }
        else
        {
            newDiff = 1.0f - yDiff;
        }

        this.character.gameObject.transform.localScale = new Vector3(newDiff, newDiff, newDiff);
    }
}
