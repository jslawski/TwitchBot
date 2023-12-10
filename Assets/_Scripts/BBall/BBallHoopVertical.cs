using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallHoopVertical : MonoBehaviour
{
    public float hoopMoveSpeed = 3f;
    private float maxYExtents = 20f;
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
            float targetYPosition = this.currentDirection * this.maxYExtents;
            Vector3 targetPosition = new Vector3(this.hoopTransform.position.x, targetYPosition, this.hoopTransform.position.z);

            this.hoopTransform.Translate((targetPosition - this.hoopTransform.position).normalized * this.hoopMoveSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(this.hoopTransform.position.y - targetPosition.y) < 0.1f)
            {
                this.hoopTransform.position = targetPosition;
                this.currentDirection *= -1f;
            }
        }
    }
}
