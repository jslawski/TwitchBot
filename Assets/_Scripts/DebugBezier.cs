using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugBezier : MonoBehaviour
{
    [SerializeField]
    private Transform point0;
    [SerializeField]
    private Transform point1;
    [SerializeField]
    private Transform point2;
    [SerializeField]
    private Transform point3;

    [SerializeField]
    private LineRenderer debugLineRenderer;

    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float tValue = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            this.DrawBezierCurve();
        }
        else
        {
            this.debugLineRenderer.positionCount = 0;
        }
    }

    private void DrawBezierCurve()
    {
        this.debugLineRenderer.positionCount = 500;

        Vector3 curvePoint = Vector3.zero;
        float testValue = 0.0f;
        //Formula for cubic Bezier curve: B(t) = (1-t)^3*P0 + 3(1-t)^2*t*P1 + 3(1-t)*t^2*P2 + t^3*P3
        for (int i = 0; i < this.debugLineRenderer.positionCount; i++)
        {
            curvePoint = (Mathf.Pow((1 - testValue), 3.0f) * this.point0.position) +
                (3 * Mathf.Pow((1 - testValue), 2.0f) * testValue * this.point1.position) +
                (3 * (1 - testValue) * Mathf.Pow(testValue, 2.0f) * this.point2.position) +
                (Mathf.Pow(testValue, 3.0f) * this.point3.position);

            this.debugLineRenderer.SetPosition(i, curvePoint);
            testValue += (1.0f / (float)this.debugLineRenderer.positionCount);
        }

        curvePoint = (Mathf.Pow((1 - tValue), 3.0f) * this.point0.position) +
                (3 * Mathf.Pow((1 - tValue), 2.0f) * tValue * this.point1.position) +
                (3 * (1 - tValue) * Mathf.Pow(tValue, 2.0f) * this.point2.position) +
                (Mathf.Pow(tValue, 3.0f) * this.point3.position);

        this.targetObject.position = curvePoint;
    }
}
