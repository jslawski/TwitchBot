using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private Transform point0;
    [SerializeField]
    private Transform point1;
    [SerializeField]
    private Transform point2;
    [SerializeField]
    private Transform point3;

    [SerializeField]
    private float moveSpeed;

    private int direction = 1;
    private float tValue = 0.0f;

    private void FixedUpdate()
    {
        this.tValue += this.moveSpeed * Time.fixedDeltaTime * direction;
        Vector3 curvePoint = this.CalculateCurvePoint();
        this.targetObject.position = curvePoint;

        if (this.tValue > 1.0f || this.tValue < 0.0f)
        {
            this.direction *= -1;
        }
    }

    private Vector3 CalculateCurvePoint()
    {
        return (Mathf.Pow((1 - this.tValue), 3.0f) * this.point0.position) +
                (3 * Mathf.Pow((1 - this.tValue), 2.0f) * this.tValue * this.point1.position) +
                (3 * (1 - this.tValue) * Mathf.Pow(this.tValue, 2.0f) * this.point2.position) +
                (Mathf.Pow(this.tValue, 3.0f) * this.point3.position);
    }
}
