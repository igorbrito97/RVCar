using System;
using UnityEngine;
using UnityEngine.UI;

public class MainCarController : MonoBehaviour
{
    internal enum WheelType
    {
        TurnInX, TurnInZ
    }
    public const int MINRPM = 1000,MAXRPM = 8000, MAXSPEED = 140; 
    private Rigidbody carRigidbody;
    public Rigidbody CarRigidbody { get => carRigidbody; set => carRigidbody = value; }
    private float horizontalInput;
    private float accelInput;
    private float brakeInput;
    private float angle;
    private float steeringAngle;
    private float initialSteeringRotation;
    private float steeringWheelAngle;
    private bool isOnGroundL;
    private bool isOnGroundR;
    private bool startButtonPressed;
    private float[,] gearRange;
    private float newRpm;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentRpm;
    [HideInInspector] public int currentGear;
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
    [SerializeField] private float power = 10000f;
    [SerializeField] private float stabilizerXspeed = 800f;
    //sound
    [SerializeField] private AudioClip highAccClip;
    private AudioSource highAccSound; 


    private void Awake()
    {
        Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
        CarRigidbody = GetComponent<Rigidbody>();
        gearRange = initGearRange();
        //rpm = rpmaux = 0;
        //marchaAtual = -1;
        //speedometer.gameObject.SetActive(true);
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

    private float[,] initGearRange()
    {
        float[,] array = new float[7,2]; //matriz, col1 = min(2k rpm), col2 = max(6k rpm)
        array[0,0] = 0.0f;
        array[0,1] = 50.0f;
        array[1,0] = 0.0f;
        array[1,1] = 25.0f;
        array[2,0] = 15.0f;
        array[2,1] = 40.0f;
        array[3,0] = 30.0f;
        array[3,1] = 65.0f;
        array[4,0] = 45.0f;
        array[4,1] = 70.0f;
        array[5,0] = 60.0f;
        array[5,1] = 90.0f;
        array[6,0] = 75.0f;
        array[6,1] = 140.0f;
        return array;
    }

    private void Start()
    {
        CarRigidbody.centerOfMass += new Vector3(0,-0.3f,-0.3f);
        isCarOn = false;
        startButtonPressed = false;
        accelInput = 0;
        brakeInput = 0;
        currentSpeed = 0;
        currentGear = -1;
        currentRpm = 0;
        newRpm = 0;
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
                CarRigidbody.AddForceAtPosition(rearLeftWC.transform.up * -antiRollForce, rearLeftWC.transform.position);
            }
            if(isOnGroundR)
            {
                CarRigidbody.AddForceAtPosition(rearRightWC.transform.up * -antiRollForce, rearRightWC.transform.position);
            }
        }        
    }

    void FixedUpdate()
    {
        if(isPaused)
        {
            frontRightWC.motorTorque = 0;
            frontLeftWC.motorTorque = 0;
            rearRightWC.motorTorque = 0;
            rearLeftWC.motorTorque = 0;
            rearRightWC.brakeTorque = 10000000;
            rearLeftWC.brakeTorque = 10000000; 
            currentSpeed = Mathf.Lerp(currentSpeed,0,Time.deltaTime*0.4f);
            currentRpm = Mathf.Lerp(currentRpm,MINRPM,Time.deltaTime*0.4f);
            Debug.Log("PAUSADO!!!!!!!!!!!");
            return;
        }
            
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            int gearAux;
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

            accelInput = (float) (rec.lY - 32768) / -65536;//rec.lY acelerador / rec.lRz freio
            brakeInput = 32767 - rec.lRz;
            int e = rec.rglSlider[0]; //embreagemSteer();
            currentSpeed = CarRigidbody.velocity.magnitude * 3.6f;
            gearAux = getActiveGear();
            Debug.Log("gearAux: " + gearAux);
            if(gearAux < 0) //ponto morto
            {
                currentGear = -1;
                Accelerate(0);
                currentRpm = Mathf.Lerp(currentRpm,MINRPM,Time.deltaTime*1.6f);
            }

            if (e < 0) //pisou na embreagem
            {
                if (gearAux > -1)// && velo > rangeMarchas[marchaEngatada,0])
                {
                    //calcula nova rotacao - se a rotacao anterior estiver baixa come�a com pouco, se tiver alta come�a com alto - ver se esta dentro da range normal
                    //coloca uma marcha verifica se a velo ta no range, se tiver abaixo reduz, se tiver acima acelera pouco, se tiver entre normal
                    if (gearAux != currentGear)
                    {
                        currentGear = gearAux;
                        newRpm = Mathf.Max(MINRPM, currentRpm * (float)currentSpeed / (float)gearRange[currentGear,1]);
                    }
                    currentRpm = Mathf.Lerp(currentRpm,newRpm,Time.deltaTime*3);
                }
                else {
                    currentRpm = Mathf.Lerp(currentRpm,MINRPM,Time.deltaTime*0.4f);
                }
            }
            else
            {
                Debug.Log("CURRENT GE: " + currentGear);
                if(currentGear > -1) //alguma esta engatada, ai coloca o rpm novo de acordo com a marcha (nos returns ali em cima colocar uma variavel de rpm novo e alterar aqui (?))
                {
                    float multiplier = CheckSpeedGear(); // retorna multiplicador -> 1 normal
                    // ai muda em uma variavel multiplicadora [0 - 1]
                    Accelerate(multiplier);
                    newRpm = MINRPM;
                    if(brakeInput > 0) //freiando
                    {
                        //Debug.Log("BRA: " + brakeInput);
                        currentRpm = Mathf.Lerp(currentRpm,MINRPM,Time.deltaTime*brakeInput*0.0002f);
                    }
                    else
                    {
                        //Debug.Log("ACCE: " + accelInput);
                        currentRpm = Mathf.Lerp(currentRpm,MAXRPM,Time.deltaTime*accelInput*0.2f);
                    }                   
                }
                else 
                {
                    currentRpm = Mathf.Lerp(currentRpm,MINRPM,Time.deltaTime*0.02f);
                }
            }
            
            
            UpdateWheelPoses();
            Stabilize();
        }

    }
    
    /*private bool estaMarchaCerta(double velo,int idx)
    {
        return LogitechGSDK.LogiButtonIsPressed(0, 11 + idx) && velo >= vetMarchas[idx].vel_ini && velo < vetMarchas[idx].vel_max;
    }*/

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

    public void Accelerate(float gearMultiplier)
    {
        if(brakeInput > 0)//is braking
        {
            rearRightWC.brakeTorque = brakeInput * brakeForce;
            rearLeftWC.brakeTorque = brakeInput * brakeForce;            
        }
        else 
        {
            int reverseMultiplier = 1;
            rearRightWC.brakeTorque = 0;
            rearLeftWC.brakeTorque = 0;

            if(LogitechGSDK.LogiButtonIsPressed(0,11))//ré
                reverseMultiplier = -1;
                
            frontRightWC.motorTorque = reverseMultiplier * accelInput * motorForce * gearMultiplier;
            frontLeftWC.motorTorque = reverseMultiplier * accelInput * motorForce * gearMultiplier;
            rearRightWC.motorTorque = reverseMultiplier * accelInput * motorForce * gearMultiplier;
            rearLeftWC.motorTorque = reverseMultiplier * accelInput * motorForce * gearMultiplier;
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
            CarRigidbody.AddForce(-transform.up * (5000 + stabilizerXspeed * Mathf.Abs((CarRigidbody.velocity.magnitude * 3.6f))));
        }
        CarRigidbody.velocity = Vector3.ClampMagnitude(CarRigidbody.velocity, 300);
    }
    private int getActiveGear()
    {
        int i = 0;
        while(i<=6)
        {
            if (LogitechGSDK.LogiButtonIsPressed(0, 11 + i))
                return i;
            i++;
        }
        return -1;
    }

    private float CheckSpeedGear()
    {
        if(currentSpeed > gearRange[currentGear,0] && currentSpeed < gearRange[currentGear,1]) //dentro do range, senao ver diferença de speed e joga pra aceleracao (multiplicador acho)
            return 1;
        //a cada 3 de diferenca de speed ele diminui x do multiplicador
        else if(currentSpeed < gearRange[currentGear,0])//abaixo -> quanto mais perto chegar da speed minima pra marcha maior valor - speed muito abaixo fica valor menor
        {
            //Debug.Log("ABAIXO!");
            return 1 - (gearRange[currentGear,0] - currentSpeed) / 3f * 0.1f;
        }
        else if(currentSpeed > gearRange[currentGear,1]) //acima -> quanto mais perto da speed maxima mais rapido - vai aumentando speed e diminui o valor
        {
            //Debug.Log("ACIMA!");
            return 1 - (currentSpeed - gearRange[currentGear,1]) / 5f * 0.1f;
        }
        else return 0;
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
