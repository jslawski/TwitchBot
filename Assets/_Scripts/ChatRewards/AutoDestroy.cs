using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public bool disableInstead = false;

    public float secondsBeforeDestruction = 3.0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("DestroyObject", this.secondsBeforeDestruction);    
    }

    private void DestroyObject()
    {
        if (this.disableInstead == true)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
