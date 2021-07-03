using UnityEngine;

public class SpinCabbage : MonoBehaviour
{
    public Rigidbody parentRigidbody;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(0, 0, -this.parentRigidbody.velocity.x));   
    }
}
