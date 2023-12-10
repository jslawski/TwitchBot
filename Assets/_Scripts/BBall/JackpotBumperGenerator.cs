using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolarCoordinates;

public class JackpotBumperGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject bumperParent;

    [SerializeField]
    private GameObject bumperPrefab;

    private List<GameObject> allBumpers;

    [SerializeField]
    private int numBumpers = 10;

    private float spawnRadius = 15.0f;

    public float currentRadius = 15f;

    [SerializeField]
    private float rotationSpeed = 0.5f;

    private float radiusUpdateSpeed = 0.5f;
    private float minRadius = 9f;
    private float maxRadius = 19f;
    private int radiusDirection = -1;

    // Start is called before the first frame update
    void Start()
    {
        this.allBumpers = new List<GameObject>();
        this.PopulateCircleOfBumpers();
    }

    private void PopulateCircleOfBumpers()
    {
        float currentAngle = 0f;
        float angleOfSeparation = (2.0f * Mathf.PI) / this.numBumpers;

        for (int i = 0; i < this.numBumpers; i++)
        {
            PolarCoordinate spawnPoint = new PolarCoordinate(spawnRadius, currentAngle, this.bumperParent.transform.position);

            GameObject newObject = Instantiate(this.bumperPrefab, spawnPoint.PolarToCartesian(), new Quaternion()) as GameObject;
            newObject.transform.parent = this.bumperParent.transform;
            this.allBumpers.Add(newObject);
            currentAngle += angleOfSeparation;
        }
    }

    private void Update()
    {
        this.UpdateBumperRadius();
    }

    private void FixedUpdate()
    {
        this.bumperParent.transform.Rotate(Vector3.forward, this.rotationSpeed);
    }

    private void UpdateBumperRadius()
    {
        this.currentRadius = 9f + Mathf.Abs(Mathf.Sin(Time.time)) * 10f;            

        for (int i = 0; i < this.allBumpers.Count; i++)
        {
            PolarCoordinate bumperPolar = PolarCoordinate.CartesianToPolar(this.allBumpers[i].transform.localPosition);
            bumperPolar.radius = this.currentRadius;
            this.allBumpers[i].transform.localPosition = bumperPolar.PolarToCartesian();
        }
    }
}
