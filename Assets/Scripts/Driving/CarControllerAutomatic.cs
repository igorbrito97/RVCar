using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerAutomatic : MonoBehaviour
{
    internal enum WheelType
    {
        TurnInX, TurnInZ
    }

    private Rigidbody carRigidbody;
    private float horizontalInput;
    private float verticalInput;
    private float accelInput;
    private float brakeInput;
    private float angle;
    private float steeringAngle;
    private float initialSteeringRotation;
    private float steeringWheelAngle;
    private float steerOldRotation;
    private bool isOnGroundL;
    private bool isOnGroundR;
    private bool startButtonPressed;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentRpm;
    [HideInInspector] public bool isCarOn;

    [SerializeField] public WheelCollider frontRightWC;
    [SerializeField] public WheelCollider frontLeftWC;
    [SerializeField] public WheelCollider rearRightWC;
    [SerializeField] public WheelCollider rearLeftWC; 
    [SerializeField] public Transform frontRightT;
    [SerializeField] public Transform frontLeftT;
    [SerializeField] public Transform rearRightT;
    [SerializeField] public Transform rearLeftT;
    [SerializeField] public float maxSteerAngle = 35f;
    [SerializeField] public float motorForce = 500f;
    [SerializeField] public float brakeForce = 500f;
    [SerializeField] private WheelType rotation = WheelType.TurnInZ;
    [SerializeField] private GameObject steeringWheel;
    [SerializeField] [Range(0.4f, 5.0f)] private float wheelSpeedTurn = 2.0f;
    [SerializeField] [Range(0.5f, 3.0f)] private float numTurns = 1.5f;
    [SerializeField] public bool reverseTurn;
    [Range(0, 1)] [SerializeField] private float steerHelper = 0.644f; // 0 is raw physics , 1 the car will grip in the direction it is facing
    [SerializeField] private float power = 10000f;
    [SerializeField] private float stabilizerXspeed = 800f;

    private void Awake()
    {
        Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
        carRigidbody = GetComponent<Rigidbody>();
        if (steeringWheel)
        {
            switch (rotation)
            {
                case WheelType.TurnInX:
                    initialSteeringRotation = steeringWheel.transform.localEulerAngles.x;
                    break;
                case WheelType.TurnInZ:
                    initialSteeringRotation = steeringWheel.transform.localEulerAngles.z;
                    break;
            }
        }
    }

    private void Start()
    {
        carRigidbody.centerOfMass += new Vector3(0,-0.3f,-0.3f);
        isCarOn = false;
        startButtonPressed = false;
        accelInput = 0;
        brakeInput = 0;
        currentSpeed = 0;
        isPaused = false;
    }

    private void Update()
    {
        if(isCarOn && !isPaused)
        {
            //stabilizer
            float powerRearLeft, powerRearRight;
            powerRearLeft = powerRearRight = 1;
            //check colission
            WheelHit hit;
            isOnGroundL = rearLeftWC.GetGroundHit(out hit);
            if(isOnGroundL)
            {
                powerRearLeft = (-rearLeftWC.transform.InverseTransformPoint(hit.point).y - rearLeftWC.radius) / rearLeftWC.suspensionDistance;
            }
            isOnGroundR = rearRightWC.GetGroundHit(out hit);
            if(isOnGroundR)
            {
                powerRearRight = (-rearRightWC.transform.InverseTransformPoint(hit.point).y - rearRightWC.radius) / rearRightWC.suspensionDistance;
            }
            //apply forces
            float antiRollForce = (powerRearLeft-powerRearRight) * power;
            if(isOnGroundL)
            {
                carRigidbody.AddForceAtPosition(rearLeftWC.transform.up * -antiRollForce, rearLeftWC.transform.position);
            }
            if(isOnGroundR)
            {
                carRigidbody.AddForceAtPosition(rearRightWC.transform.up * -antiRollForce, rearRightWC.transform.position);
            }
        }        
    }

    void FixedUpdate()
    {
        if(isPaused)
            return;
            
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            //float h, v;, velo;
            //int e, marchaEngatada, op;
            //bool flagMarcha = false;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            horizontalInput = (float) rec.lX / 32768;//volante
            Steer();
            TurnSteeringWheel();

            if(!isCarOn) //se carro estiver desligado não faz nada, só mexe o volante
            {
                CheckCarStart();
                return;
            }

            accelInput = (float) (rec.lY - 32768) / -65536;
            brakeInput = 32767 - rec.lRz;
            //verticalInput = (float)(rec.lY - rec.lRz) / -65534; //rec.lY acelerador / rec.lRz freio
            int e = rec.rglSlider[0]; //embreagemSteer();
            currentSpeed = carRigidbody.velocity.magnitude * 3.6f;
            Accelerate();
            UpdateWheelPoses();
            Stabilize();
        }
    }

    private void CheckCarStart()
    {
        if(LogitechGSDK.LogiButtonIsPressed(0, 10)) //start button
            startButtonPressed = true;

        if(startButtonPressed)
        {
            currentRpm = Mathf.Lerp(currentRpm,1000,Time.deltaTime*7f); 
            if(currentRpm > 995) //close enough
            {
                isCarOn = true;
                currentRpm = 1000;
            }
        }
    }

     public void Steer()
    {
        steeringAngle = horizontalInput*maxSteerAngle;
        frontRightWC.steerAngle = steeringAngle;
        frontLeftWC.steerAngle = steeringAngle;
    }

    public void Accelerate()
    {
        if(brakeInput > 0)//is braking
        {
            rearRightWC.brakeTorque = brakeInput * brakeForce;
            rearLeftWC.brakeTorque = brakeInput * brakeForce;            
        }
        else 
        {
            int multiplier = 1;
            rearRightWC.brakeTorque = 0;
            rearLeftWC.brakeTorque = 0;

            if(LogitechGSDK.LogiButtonIsPressed(0,11))//ré
                multiplier = -1;
                
            frontRightWC.motorTorque = multiplier * accelInput * motorForce;
            frontLeftWC.motorTorque = multiplier * accelInput * motorForce;
            rearRightWC.motorTorque = multiplier * accelInput * motorForce;
            rearLeftWC.motorTorque = multiplier * accelInput * motorForce;
        }
    }

    public void UpdateWheelPoses()
    {
        UpdateWheelPose(frontRightWC,frontRightT);
        UpdateWheelPose(frontLeftWC,frontLeftT);
        UpdateWheelPose(rearRightWC,rearRightT);
        UpdateWheelPose(rearLeftWC,rearLeftT);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform transf)
    {
        Vector3 pos = transf.position;
        Quaternion quat = transf.rotation;

        collider.GetWorldPose(out pos, out quat);
        transf.position = pos;
        transf.rotation = quat;
    }

    private void Stabilize()
    {
        if(isOnGroundL || isOnGroundR) 
        {
            carRigidbody.AddForce(-transform.up * (5000 + stabilizerXspeed * Mathf.Abs((carRigidbody.velocity.magnitude * 3.6f))));
        }
        carRigidbody.velocity = Vector3.ClampMagnitude(carRigidbody.velocity, 300);
    }

    private void TurnSteeringWheel()
    {
        if (steeringWheel)
        {
            int speedMultiplier = reverseTurn ? -1 : 1;
            float rotX, rotY, rotZ, wheelDirection = horizontalInput * speedMultiplier;
            steeringWheelAngle = Mathf.MoveTowards(steeringWheelAngle, wheelDirection, Time.deltaTime * wheelSpeedTurn);
            rotX = steeringWheel.transform.localEulerAngles.x;
            rotY = steeringWheel.transform.localEulerAngles.y;
            rotZ = steeringWheel.transform.localEulerAngles.z;
            switch (rotation)
            {
                case WheelType.TurnInX:
                    steeringWheel.transform.localEulerAngles = new Vector3(rotX, initialSteeringRotation + (steeringWheelAngle * numTurns * 360), rotZ);
                    break;
                case WheelType.TurnInZ:
                    steeringWheel.transform.localEulerAngles = new Vector3(rotX, rotY, initialSteeringRotation + (steeringWheelAngle * numTurns * 360));
                    break;

            }
        }
    }


}
