//--------------------------------------------------------------
//      Vehicle Physics Pro: advanced vehicle physics kit
//          Copyright © 2011-2019 Angel Garcia "Edy"
//        http://vehiclephysics.com | @VehiclePhysics
//--------------------------------------------------------------

// Dashboard: handles a dashboard UI with signals and gauges


using UnityEngine;
using UnityEngine.UI;
using System;


namespace VehiclePhysics.UI
{

public class Dashboard : MonoBehaviour
	{

		[SerializeField] Font font;
	public MainCarController carController;

	[Header("Needles")]
	public Needle speedNeedle = new Needle();
	public Needle rpmNeedle = new Needle();

	[Header("UI labels")]
	public Text gearLabel;

	[Serializable]
	public class Needle
		{
		public Transform needle;
		public float minValue = 0.0f;
		public float maxValue = 200.0f;
		public float angleAtMinValue = 135.0f;
		public float angleAtMaxValue = -135.0f;

		public void SetValue (float value)
			{
			if (needle == null) return;

			float x = (value - minValue) / (maxValue - minValue);
			float angle = Mathf.LerpUnclamped(angleAtMinValue, angleAtMaxValue, x);
			//float angle = MathUtility.UnclampedLerp(angleAtMinValue, angleAtMaxValue, x);

			needle.localRotation = Quaternion.Euler(0, 0, angle);
			}
		}


	float m_lastVehicleTime;
	float m_warningTime;

	// são os valores que são colocados na UI - velocidade (0-200) e rpm (0-7000)
	private float m_speedMs; 
	private float m_engineRpm;


	void OnEnable ()
		{
		//m_lastVehicleTime = -1.0f;
		//m_warningTime = -10.0f;
		
		//instanciar o carro
		/*
		if (carController == null)
			GameObject.Find(LevelManager.Instance.currentSession.Name);
			carController = GetComponentInParent<MainCarController>();*/
		}


	void FixedUpdate ()
		{/*
		if (vehicle == null || !vehicle.isActiveAndEnabled) return;*/

		//m_speedMs.Set(vehicle.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000.0f);
		//m_engineRpm.Set(vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000.0f);
			m_speedMs = 20f;
			m_engineRpm = 1000f;
		}


	void Update ()
		{/*
		if (vehicle == null || !vehicle.isActiveAndEnabled)
			{
			speedNeedle.SetValue(0.0f);
			rpmNeedle.SetValue(0.0f);
			m_speedMs.Reset(0.0f);
			m_engineRpm.Reset(0.0f);

			if (gearLabel != null) gearLabel.text = "-";
			if (speedMphLabel != null) speedMphLabel.text = "-";

			m_lastVehicleTime = -1.0f;
			return;
			}

		// Needles use interpolated values computed at visual frame rate

		float frameRatio = InterpolatedFloat.GetFrameRatio();

		float speedMs = m_speedMs.GetInterpolated(frameRatio);
		float engineRpm = m_engineRpm.GetInterpolated(frameRatio);
		if (speedMs < 0) speedMs = 0.0f;
		if (engineRpm < 0) engineRpm = 0.0f;*/

		speedNeedle.SetValue(m_speedMs);//speedMs * 3.6f);
		rpmNeedle.SetValue(m_engineRpm);


			// Gear label

			if (gearLabel != null)
				{
				int gearId = 3;//marcha
				int gearMode = 0;// se é manual 0 ou automatico 1
				bool switchingGear = false;//se esta mudando de marcha
				gearLabel.font = font;

				switch (gearMode)
					{
					case 0:		// M
						if (gearId == 0)
							gearLabel.text = switchingGear? " " : "N";
						else
						if (gearId > 0)
							gearLabel.text = gearId.ToString();
						else
							{
							if (gearId == -1)
								gearLabel.text = "R";
							else
								gearLabel.text = "R" + (-gearId).ToString();
							}
						break;

					case 1:		// P
						gearLabel.text = "P";
						break;
						/*
					case 2:		// R
						if (gearId < -1)
							gearLabel.text = "R" + (-gearId).ToString();
						else
							gearLabel.text = "R";
						break;

					case 3:		// N
						gearLabel.text = "N";
						break;

					case 4:		// D
						if (gearId > 0)
							gearLabel.text = "D" + gearId.ToString();
						else
							gearLabel.text = "D";
						break;

					case 5:		// L
						if (gearId > 0)
							gearLabel.text = "L" + gearId.ToString();
						else
							gearLabel.text = "L";
						break;
						*/
					default:
						gearLabel.text = "-";
						break;
					}
				}


			//m_lastVehicleTime = vehicle.time;
			//}
		}


	void SetEnabled (GameObject go, bool active)
		{
		if (go != null) go.SetActive(active);
		}
	}

}
