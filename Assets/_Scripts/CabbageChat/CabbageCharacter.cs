using UnityEngine;
using CharacterCustomizer;

public class CabbageCharacter : MonoBehaviour
{
    public CustomCharacter character;

    private string username;

    public void UpdateCharacter(string username)
    {
        this.username = username;

        if (CharacterCache.IsCached(this.username))
        {
            this.character.LoadCharacterFromJSON(CharacterCache.GetCachedSettings(this.username));

            if (this.ShouldScaleCharacter() == true)
            {
                this.ScaleCharacter();
            }
        }
        else
        {
            GetCurrentPresetAsyncRequest request = new GetCurrentPresetAsyncRequest(this.username, this.GetCurrentPresetSuccess, this.GetCurrentPresetFailure);
            request.Send();
        }
    }

    private void GetCurrentPresetSuccess(string data)
    {
        if (this.character == null)
        {
            return;
        }

        this.character.LoadCharacterFromJSON(data);

        //Cache Attribute Settings
        CharacterCache.UpdateCache(this.username, data);

        if (this.ShouldScaleCharacter() == true)
        {
            this.ScaleCharacter();
        }
    }

    private void GetCurrentPresetFailure()
    {
        Debug.LogError("Error: Unable to fetch user's current preset");
    }

    private bool ShouldScaleCharacter()
    {
        //return false;

        CharacterAttribute baseAttribute = this.character.GetAttribute(AttributeType.BaseCabbage);
        float scaleX = baseAttribute.GetScaleX();
        float scaleY = baseAttribute.GetScaleY();

        return (scaleX > 1.0f || scaleY > 1.0f);

        return ((scaleX < 1.0f && scaleY < 1.0f) || (scaleX > 1.0f || scaleY > 1.0f));
    }

    private void ScaleCharacter()
    {
        float adjustment = 1.0f;

        CharacterAttribute baseAttribute = this.character.GetAttribute(AttributeType.BaseCabbage);
        float scaleX = baseAttribute.GetScaleX();
        float scaleY = baseAttribute.GetScaleY();

        if (scaleX > scaleY)
        {
            adjustment = 1.0f / scaleX;
        }
        else
        {
            adjustment = 1.0f / scaleY;
        }
        
        this.character.gameObject.transform.localScale = new Vector3(adjustment, adjustment, adjustment);
    }

    private void OnDestroy()
    {
        CharacterCache.ClearCacheForUser(this.username);
    }
}
