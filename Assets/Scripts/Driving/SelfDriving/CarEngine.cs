using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFR;
    public WheelCollider wheelFL;
    public WheelCollider wheelRR;
    public WheelCollider wheelRL;
    public float maxMotorTorque = 80f;
    public float maxBreakTorque = 150f;
    public float currentSpeed;
    public float maxSpeed = 100f;  
    public Vector3 centerOfMass;
    public bool isBraking = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;

    private List<Transform> nodes;
    private int currentNode = 0;
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        Transform[] pathTranform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for(int i = 0; i< pathTranform.Length; i++)
        {
            if(pathTranform[i] != path.transform)
                nodes.Add(pathTranform[i]);
        }
    }
    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckNextNodeDistance();
        Breaking();
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFR.radius * wheelFR.rpm * 60 / 1000;
        if(currentSpeed < maxSpeed && !isBraking)
        {
            wheelFR.motorTorque = maxMotorTorque;
            wheelFL.motorTorque = maxMotorTorque;
        }
        else {
            wheelFR.motorTorque = 0;
            wheelFL.motorTorque = 0;
        }
    }

    private void CheckNextNodeDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            if(currentNode == nodes.Count -1)
                currentNode = 0;
            else 
                currentNode++;
        }
    }

    private void Breaking()
    {
        if(isBraking)
        {
            carRenderer.material.mainTexture = textureBraking;
            wheelRL.brakeTorque = maxBreakTorque;
            wheelRR.brakeTorque = maxBreakTorque;
        }
        else {
            carRenderer.material.mainTexture = textureNormal;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }
}
