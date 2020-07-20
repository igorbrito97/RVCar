using System;
using UnityEngine;
using UnityEngine.UI;

public class MainCarController : MonoBehaviour
{
    /*
    public int marchaAtual { get; private set; }
    public const int MINRPM = 0,MAXRPM = 8000; 
    [SerializeField] Text speedometer;
    [SerializeField] Text rotationsPerMinute;

    private float[,] rangeMarchas;
    private float rotacaoInicialVolante = 0.0f;
    private float anguloVolante;
    private int rpm,rpmaux; //0 - 8000
    private float brakerate = 0,accelrate = 0;
    */


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
        //rangeMarchas = initRangeMarcha();
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

    private float[,] initRangeMarcha()
    {
        float[,] array = new float[7,2]; //matriz, col1 = min(2k rpm), col2 = max(5k rpm)
        array[0,0] = 0.0f;
        array[0,1] = 50.0f;
        array[1,0] = 0.0f;
        array[1,1] = 20.0f;
        array[2,0] = 10.0f;
        array[2,1] = 40.0f;
        array[3,0] = 25.0f;
        array[3,1] = 50.0f;
        array[4,0] = 40.0f;
        array[4,1] = 65.0f;
        array[5,0] = 50.0f;
        array[5,1] = 90.0f;
        array[6,0] = 60.0f;
        array[6,1] = 110.0f;
        return array;
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
            /*
            // velo = m_Car.CurrentSpeed;
            //marchaEngatada = getMarchaEngatada();
            if (e < 0)
            {
                rpmaux = rpm > rpmaux ? rpm : rpmaux; //pega o maior rpm depois de pisar na embreagem
                rpm = changeRpm(-7, 1);//rotacao cai
                if (marchaEngatada > -1)// && velo > rangeMarchas[marchaEngatada,0])
                {
                    //calcula nova rotacao - se a rotacao anterior estiver baixa come�a com pouco, se tiver alta come�a com alto - ver se esta dentro da raange normal
                    //coloca uma marcha verifica se a velo ta no range, se tiver abaixo reduz, se tiver acima acelera pouco, se tiver entre normal
                    if (marchaAtual != marchaEngatada)
                    {
                        
                        if (velo > rangeMarchas[marchaEngatada, 0] && velo < rangeMarchas[marchaEngatada, 1]) //velocidade normal - rotacao normal
                            op = 0;
                        else if (velo > rangeMarchas[marchaEngatada, 1]) //velocidade acima do range - rotacao fica alta
                            op = 1;
                        else //velocidade abaixo do range - rotacao fica baixa
                            op = 2;
                        //Debug.Log("op = " + op);
                        //Debug.Log("marcha atual = " + marchaAtual + " marcha engatada = " + marchaEngatada);
                        rpm = newRpm(op, marchaAtual - marchaEngatada, rpmaux);
                        marchaAtual = marchaEngatada;
                        
                    }
                }

            }
            else
            {
                rpmaux = 0;
                if (marchaAtual == marchaEngatada) //engatou a marcha
                {
                    if (marchaAtual == 0) // ré
                        v *= -1;

                    if (v < 0)//freiando
                    {
                        brakerate -= 0.1f;
                        rpm = changeRpm(-10, v * brakerate);
                        //Debug.Log("brakerate = "+ brakerate + " v = " + v);
                    }
                    else
                    {
                        brakerate = 0;
                        rpm = changeRpm(10, v);
                    }

                }
                else if (marchaEngatada == -1)//ponto morto
                {

                }

            }
            // o que fazer = reduzir drasticamente ou esgasgar o motor / deixar acelerar devagar [marchas erradas] & manter marcha nao engatada & r�
            //quando freia tem que perder rpm tbm

            /*
            if (marchaEngatada >= 0)
                m_Car.Move(h, v, v, 0);
            else
                m_Car.Move(h, 0, 0, 0);// o ultimo � o freio de mao
            */

            //Debug.Log("velo = " + velo);
            //Debug.Log("marcha atual = " + marchaAtual + "marcha engatada = " + marchaEngatada);
            //Debug.Log("h = " + h + "v = " + v);
            //moverVolante(h);
            //speedometer.text = string.Format("{0,00}", velo) + " - " + marchaAtual;
            //rotationsPerMinute.text = "rpm = " + rpm;
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

    /*

    private int changeRpm(int mult,float v) 
    {
        if (mult > 0) //v nao pode ser zero - (se nao acelerar)
            v = v == 0 ? -0.5f : v;
        rpm += Convert.ToInt32(mult * v);

        if (rpm < MINRPM)
            return MINRPM;
        else if (rpm > MAXRPM)
            return MAXRPM;
        else return rpm;
    }

    private int newRpm(int op,int dif,int aux)
    {
        Debug.Log("dif = " + dif + " aux = " + aux);
        switch(op)//coloca uma marcha verifica se a velo ta no range, se tiver abaixo reduz, se tiver acima acelera pouco, se tiver entre normal
        {
            case 0://entre
                rpm /= 2;
                break;
            case 1://abaixo
                rpm = Convert.ToInt32(aux * -1.5 * dif);
                break;
            case 2://acima
                rpm = Convert.ToInt32(aux / -1.5 / dif);
                break;
        }
        if (dif > 0) //up
            return (rpm > MAXRPM) ? MAXRPM : rpm;
        else //down
            return (rpm < MINRPM) ? MINRPM : rpm;
    }

    private void checkBrake(float velo)
    {

    }

    private int getMarchaEngatada()
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
    */

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
