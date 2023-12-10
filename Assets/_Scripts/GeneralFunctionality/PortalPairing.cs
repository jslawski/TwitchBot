using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPairing : MonoBehaviour
{
    private BoxCollider[] portalColliders;

    private float deactivateTime = 0.5f;

    [SerializeField]
    private AudioSource portalSound;

    // Start is called before the first frame update
    void Awake()
    {
        this.portalColliders = GetComponentsInChildren<BoxCollider>();
    }

    public void TeleportGameObject(int portalIndex, GameObject teleportedObject)
    {
        switch (portalIndex)
        {
            case 0:
                teleportedObject.transform.position = this.portalColliders[1].transform.position;
                break;
            case 1:
                teleportedObject.transform.position = this.portalColliders[0].transform.position;
                break;
            default:
                Debug.LogError("Invalid Portal Index: " + portalIndex);
                break;
        }

        StopAllCoroutines();
        StartCoroutine(DeactivatePortals());
    }

    private IEnumerator DeactivatePortals()
    {
        this.portalSound.Play();

        for (int i = 0; i < this.portalColliders.Length; i++)
        {
            this.portalColliders[i].enabled = false;
        }

        yield return new WaitForSeconds(this.deactivateTime);

        for (int i = 0; i < this.portalColliders.Length; i++)
        {
            this.portalColliders[i].enabled = true;
        }
    }
}
