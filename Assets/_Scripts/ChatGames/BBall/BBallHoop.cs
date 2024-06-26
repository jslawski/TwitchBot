﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBallHoop : MonoBehaviour
{
    public float hoopMoveSpeed = 3f;
    private float maxXExtents = 20f;
    private float currentDirection = -1f;
    [SerializeField]
    private Transform hoopTransform;

    public bool vertical = false;
    private float maxYExtents = 20f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.vertical == false)
        {
            float targetXPosition = this.currentDirection * this.maxXExtents;
            Vector3 targetPosition = new Vector3(targetXPosition, this.hoopTransform.position.y, this.hoopTransform.position.z);

            this.hoopTransform.Translate((targetPosition - this.hoopTransform.position).normalized * this.hoopMoveSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(this.hoopTransform.position.x - targetPosition.x) < 0.1f)
            {
                this.hoopTransform.position = targetPosition;
                this.currentDirection *= -1f;
            }
        }
        else
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
