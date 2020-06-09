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
    private float angle;
    private float steeringAngle;
    private float initialSteeringRotation = 0.0f;
    private float steeringWheelAngle;
    private float steerOldRotation;

    [SerializeField] public WheelCollider frontRightWC;
    [SerializeField] public WheelCollider frontLeftWC;
    [SerializeField] public WheelCollider rearRightWC;
    [SerializeField] public WheelCollider rearLeftWC; 
    [SerializeField] public Transform frontRightT;
    [SerializeField] public Transform frontLeftT;
    [SerializeField] public Transform rearRightT;
    [SerializeField] public Transform rearLeftT;
    [SerializeField] public float maxSteerAngle = 30f;
    [SerializeField] public float motorForce = 50f;
    [SerializeField] private WheelType rotation = WheelType.TurnInZ;
    [SerializeField] private GameObject steeringWheel;
    [SerializeField] [Range(0.4f, 5.0f)] private float wheelSpeedTurn = 2.0f;
    [SerializeField] [Range(0.5f, 3.0f)] private float numTurns = 1.5f;
    [SerializeField] bool reverseTurn;
    [Range(0, 1)] [SerializeField] private float steerHelper = 0.644f; // 0 is raw physics , 1 the car will grip in the direction it is facing

    private void Awake()
    {
        Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            //GetInput();
            LogitechGSDK.DIJOYSTATE2ENGINES rec = LogitechGSDK.LogiGetStateUnity(0);
            horizontalInput = (float) rec.lX / 32768;//volante
            verticalInput = (float) (rec.lY - rec.lRz) / -65534; //rec.lY acelerador / rec.lRz freio
            Steer();
            //SteerHelper();
            Accelerate();
            UpdateWheelPoses();
            TurnSteeringWheel();
        }
    }
    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
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
        steeringAngle = maxSteerAngle * horizontalInput;

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

    private void TurnSteeringWheel()
    {
        if (steeringWheel)
        {
            int speedMultiplier = reverseTurn ? -1 : 1;
            float rotX, rotY, rotZ, wheelDirection = steeringAngle * speedMultiplier;
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
