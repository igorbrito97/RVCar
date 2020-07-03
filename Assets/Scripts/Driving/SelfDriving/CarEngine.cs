using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
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

    [Header("Sensors")] //valores mudam de acordo com o carro
    public float sensorLength = 8f;
    public Vector3 frontSensorPosition = new Vector3(0, 0.2f, 0.5f);
    public float frontSideSensorPosition = 1.4f;
    public float frontSensorAngle = 30f;

    private List<Transform> nodes;
    private int currentNode = 0;
    private bool isAvoiding = false;
    private float targetSteerAngle = 0;
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
        CheckSensors();
        ApplySteer();
        Drive();
        CheckNextNodeDistance();
        Breaking();
        LerpToSteerAngle();
    }

    private void CheckSensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

        float avoidTurn = 0;
        isAvoiding = false;


        //front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn -= 1f;
            }
        }

        //front right angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn -= 0.5f;
            }
        }

        //front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn += 1f;

            }
        }

        //front left angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn += 0.5f;
            }
        }

        //front center sensor
        if(avoidTurn == 0)
        {
            if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if(hit.collider.CompareTag("Environment"))
                {
                    Debug.DrawLine(sensorStartPos,hit.point);
                    isAvoiding = true;
                    if(hit.normal.x < 0)
                        avoidTurn = -1f;
                    else
                        avoidTurn = 1f;
                }
            }
        }

        Debug.Log("AVOID TURN: " + avoidTurn);
        Debug.Log(" ISAV " + isAvoiding);
        if(isAvoiding)
            targetSteerAngle = maxSteerAngle * avoidTurn;

    }


    private void ApplySteer()
    {
        if(isAvoiding)
            return;
            
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
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

    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
}
