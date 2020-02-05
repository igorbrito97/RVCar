using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        internal enum tipoVolante
        {
            GirarEmX, GirarEmZ
        }

        public int marchaAtual { get; private set; }
        public const int MINRPM = 0,MAXRPM = 8000; 

        [SerializeField] private tipoVolante rotacao = tipoVolante.GirarEmZ;
        [SerializeField] private GameObject volante;
        [SerializeField] [Range(0.4f, 5.0f)] private float velGiroVolante = 2.0f;
        [SerializeField] [Range(0.5f, 3.0f)] private float numVoltas = 1.5f;
        [SerializeField] bool inverterGiro;
        [SerializeField] Text speedometer;
        [SerializeField] Text rotationsPerMinute;

        private CarController m_Car; // the car controller we want to use
        private float[,] rangeMarchas;
        private float rotacaoInicialVolante = 0.0f;
        private float anguloVolante;
        private int rpm,rpmaux; //0 - 8000
        private float brakerate = 0,accelrate = 0;




        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
            rangeMarchas = initRangeMarcha();
            rpm = rpmaux = 0;
            marchaAtual = -1;
            speedometer.gameObject.SetActive(true);
            if (volante)
            {
                switch (rotacao)
                {
                    case tipoVolante.GirarEmX:
                        rotacaoInicialVolante = volante.transform.localEulerAngles.x;
                        break;
                    case tipoVolante.GirarEmZ:
                        rotacaoInicialVolante = volante.transform.localEulerAngles.z;
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

        void FixedUpdate()
        {
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                float h, v, velo;
                int e, marchaEngatada, op;
                //bool flagMarcha = false;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                h = (float)rec.lX / 32768;//volante
                v = (float)(rec.lY - rec.lRz) / -65534; //rec.lY acelerador / rec.lRz freio
                e = rec.rglSlider[0]; //embreagem
                velo = m_Car.CurrentSpeed;
                marchaEngatada = getMarchaEngatada();
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

                if (marchaEngatada >= 0)
                    m_Car.Move(h, v, v, 0);
                else
                    m_Car.Move(h, 0, 0, 0);// o ultimo � o freio de mao


                //Debug.Log("velo = " + velo);
                //Debug.Log("marcha atual = " + marchaAtual + "marcha engatada = " + marchaEngatada);
                //Debug.Log("h = " + h + "v = " + v);
                moverVolante(h);
                speedometer.text = string.Format("{0,00}", velo) + " - " + marchaAtual;
                rotationsPerMinute.text = "rpm = " + rpm;
            }
        }
        
        /*private bool estaMarchaCerta(double velo,int idx)
        {
            return LogitechGSDK.LogiButtonIsPressed(0, 11 + idx) && velo >= vetMarchas[idx].vel_ini && velo < vetMarchas[idx].vel_max;
        }*/

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

        private void moverVolante(float h)
        {
            if (volante)
            {
                int velMult = inverterGiro ? -1 : 1;
                float rotX, rotY, rotZ, direcaoVolante = h * velMult;
                anguloVolante = Mathf.MoveTowards(anguloVolante, direcaoVolante, Time.deltaTime * velGiroVolante);
                rotX = volante.transform.localEulerAngles.x;
                rotY = volante.transform.localEulerAngles.y;
                rotZ = volante.transform.localEulerAngles.z;
                switch (rotacao)
                {
                    case tipoVolante.GirarEmX:
                        volante.transform.localEulerAngles = new Vector3(rotX, rotacaoInicialVolante + (anguloVolante * numVoltas * 360), rotZ);
                        break;
                    case tipoVolante.GirarEmZ:
                        volante.transform.localEulerAngles = new Vector3(rotX, rotY, rotacaoInicialVolante + (anguloVolante * numVoltas * 360));
                        break;

                }
            }
        }

    }
}

// VOU PISANDO E VAI AUMENTANDO O RPM CONSTANTEMENTE - QUAL A RELA��O DE RPM E VELOCIDADE