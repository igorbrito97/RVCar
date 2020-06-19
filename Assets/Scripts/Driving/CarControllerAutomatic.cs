﻿using System;
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
    private float angle;
    private float steeringAngle;
    private float initialSteeringRotation = 0.0f;
    private float steeringWheelAngle;
    private float steerOldRotation;
    private bool isOnGroundL;
    private bool isOnGroundR;

    [SerializeField] public WheelCollider frontRightWC;
    [SerializeField] public WheelCollider frontLeftWC;
    [SerializeField] public WheelCollider rearRightWC;
    [SerializeField] public WheelCollider rearLeftWC; 
    [SerializeField] public Transform frontRightT;
    [SerializeField] public Transform frontLeftT;
    [SerializeField] public Transform rearRightT;
    [SerializeField] public Transform rearLeftT;
    [SerializeField] public float maxSteerAngle = 35f;
    [SerializeField] public float motorForce = 50f;
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
    }

    private void Update()
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

    private void FixedUpdate()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            //GetInput();
            LogitechGSDK.DIJOYSTATE2ENGINES rec = LogitechGSDK.LogiGetStateUnity(0);
            horizontalInput = (float) rec.lX;//rec.lX / 32768;//volante
            verticalInput = (float) (rec.lY - rec.lRz) / -65534; //rec.lY acelerador / rec.lRz freio
            Steer();
            //SteerHelper();
            Accelerate();
            UpdateWheelPoses();
            Stabilize();
            TurnSteeringWheel();
        }
    }
    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Debug.Log("H: " + horizontalInput + " V: " + verticalInput);
    }

    public void Steer()
    {
        //se estiver pressionando tudo ele vai de um ponto até outro em tanto tempo - suaviza
        if(horizontalInput > 0.7f || horizontalInput < -0.7f) {
            angle = Mathf.Lerp(angle, horizontalInput, Time.deltaTime*4);
        } 
        else {
            angle = Mathf.Lerp(angle, horizontalInput, Time.deltaTime*2);
        }
        // OU
        steeringAngle = horizontalInput/450;

        //por enquanto estou usando o steering, ver se suaviza mesmo com o angle
        Debug.Log("VER:" + verticalInput);
        Debug.Log("Steering + " + steeringAngle + " ANGLEEEE: " + angle * maxSteerAngle);
        frontRightWC.steerAngle = steeringAngle;
        frontLeftWC.steerAngle = steeringAngle;
    }

    public void Accelerate()
    {
        frontRightWC.motorTorque = verticalInput * motorForce;
        frontLeftWC.motorTorque = verticalInput * motorForce;
        rearRightWC.motorTorque = verticalInput * motorForce;
        rearLeftWC.motorTorque = verticalInput * motorForce;
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

    private void SteerHelper()
    {
        WheelHit wheelHitFR, wheelHitFL, wheelHitRR, wheelHitRL;
        // wheels arent on the ground so dont realign the rigidbody velocity
        frontRightWC.GetGroundHit(out wheelHitFR);
        if(wheelHitFR.normal == Vector3.zero)
            return;
        frontLeftWC.GetGroundHit(out wheelHitFL);
        if(wheelHitFL.normal == Vector3.zero)
            return;
        rearRightWC.GetGroundHit(out wheelHitRR);
        if(wheelHitRR.normal == Vector3.zero)
            return;
        rearLeftWC.GetGroundHit(out wheelHitRL);
        if(wheelHitRL.normal == Vector3.zero)
            return;

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(steerOldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - steerOldRotation) * steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            carRigidbody.velocity = velRotation * carRigidbody.velocity;
        }
        steerOldRotation = transform.eulerAngles.y;
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
