using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private int portalIndex;

    [SerializeField]
    private PortalPairing portalParent;


    private void OnTriggerEnter(Collider other)
    {
        this.portalParent.TeleportGameObject(portalIndex, other.gameObject);
    }
}
