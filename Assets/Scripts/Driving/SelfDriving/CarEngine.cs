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
    private int currentNode;
    private bool isAvoiding = false;
    private float targetSteerAngle = 0;
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        path = GameObject.Find("Path").transform; //pega da cena
        Transform[] pathTranform = path.GetComponentsInChildren<Transform>();
        //ver qual é o ponto mais proximo do carro e começar a colocar os nodes a partir dele, ai vai seguir um caminho só
        
        /*
        int pos = GetClosestNode(pathTranform);
        nodes = new List<Transform>();
        for(int count = 0; count < pathTranform.Length; count ++)
        {
            if(pathTranform[pos] != path.transform)
            {
                nodes.Add(pathTranform[pos]);
            }
            if(pos == pathTranform.Length-1) //last one
                pos = 0;
            else pos++;
        }*/

        currentNode = GetFirstNode(pathTranform);
        nodes = new List<Transform>();
        for(int i = 0; i< pathTranform.Length; i++)
        {
            if(pathTranform[i] != path.transform)
                nodes.Add(pathTranform[i]);
        }
        Debug.Log("FIRST NODE: "+ currentNode);
        Debug.Log("NODES COUNT: " + nodes.Count);
        
    }

    private int GetFirstNode(Transform[] pathTranform)
    {
        float minDist = Vector3.Distance(this.transform.position, pathTranform[1].position);
        int pos = 1, i = 2;
        while(i < pathTranform.Length)// pos 0 não
        {
            if(Vector3.Distance(this.transform.position, pathTranform[i].position) < minDist)
            {
                pos = i;
                minDist = Vector3.Distance(this.transform.position, pathTranform[i].position);
            }
            i++;
        }
        return pos-1;
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
            if(hit.collider.CompareTag("Environment") || hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn -= 1f;
            }
        }

        //front right angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))// || hit.collider.CompareTag("Car"))
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
            if(hit.collider.CompareTag("Environment") || hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(sensorStartPos,hit.point);
                isAvoiding = true;
                avoidTurn += 1f;

            }
        }

        //front left angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if(hit.collider.CompareTag("Environment"))// || hit.collider.CompareTag("Car"))
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
                if(hit.collider.CompareTag("Environment"))// || hit.collider.CompareTag("Car"))
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
        //Debug.Log("TIME: " + (maxMotorTorque * Time.deltaTime * 90));
        currentSpeed = 2 * Mathf.PI * wheelFR.radius * wheelFR.rpm * 60 / 1000;
        if(currentSpeed < maxSpeed && !isBraking)
        {
            wheelFR.motorTorque = maxMotorTorque * Time.deltaTime * 90;
            wheelFL.motorTorque = maxMotorTorque * Time.deltaTime * 90;
        }
        else {
            wheelFR.motorTorque = 0;
            wheelFL.motorTorque = 0;
        }
    }

    private void CheckNextNodeDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.4f)
        {
            if(nodes[currentNode].CompareTag("Brake"))
            {
                isBraking = true;
            }
            else isBraking = false;

            //Debug.Log("CHEGOU PERTO: " + currentNode);
            if(currentNode == nodes.Count -1)
                currentNode = 0;
            else 
                currentNode++;
            
            Debug.Log("próximo: " + currentNode + " - " + this.gameObject.name);
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
