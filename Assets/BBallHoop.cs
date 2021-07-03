using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallHoop : MonoBehaviour
{
    public float hoopMoveSpeed = 3f;
    private float maxXExtents = 20f;
    private float currentDirection = -1f;
    [SerializeField]
    private Transform hoopTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ChatManager.shootModeActive)
        {
            float targetXPosition = this.currentDirection * this.maxXExtents;
            Vector3 targetPosition = new Vector3(targetXPosition, this.hoopTransform.position.y, this.hoopTransform.position.z);

            this.hoopTransform.Translate((targetPosition - this.hoopTransform.position).normalized * this.hoopMoveSpeed * Time.fixedDeltaTime);
            //this.hoopTransform.position = Vector3.Lerp(this.hoopTransform.position, targetPosition, Time.fixedDeltaTime * this.hoopMoveSpeed);
            if (Mathf.Abs(this.hoopTransform.position.x - targetPosition.x) < 0.1f)
            {
                this.hoopTransform.position = targetPosition;
                this.currentDirection *= -1f;
            }
        }
    }
}
