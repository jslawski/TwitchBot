using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    void FixedUpdate()
    {
        this.gameObject.transform.Rotate(-Vector3.forward, rotationSpeed * Time.fixedDeltaTime);        
    }
}
