using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateJackpot : MonoBehaviour
{
    [SerializeField]
    private Transform parentTransform;

    private float rotationSpeed = 0.5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.parentTransform.Rotate(Vector3.forward, this.rotationSpeed);
    }
}
