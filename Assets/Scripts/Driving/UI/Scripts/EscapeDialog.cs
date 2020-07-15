//--------------------------------------------------------------
//      Vehicle Physics Pro: advanced vehicle physics kit
//          Copyright © 2011-2019 Angel Garcia "Edy"
//        http://vehiclephysics.com | @VehiclePhysics
//--------------------------------------------------------------

// EscapeDialog: controls the Escape dialog and its options


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class EscapeDialog : MonoBehaviour
	{
	public MainCarController carController;
	public KeyCode escapeKey = KeyCode.Escape;

	[Header("Buttons")]
	public Button continueButton;
	public Button resetButton;
	public Button quitButton;

	void Start()
	{
		carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
	}

	void OnEnable ()
	{
		AddListener(continueButton, OnContinue);
		AddListener(resetButton, OnReset);
		AddListener(quitButton, OnQuit);
	}
	void OnDisable ()
	{
		RemoveListener(continueButton, OnContinue);
		RemoveListener(resetButton, OnReset);
		RemoveListener(quitButton, OnQuit);
	}
		

	void Update ()
	{
		if (Input.GetKeyDown(escapeKey))
		{
			carController.isPaused = false;
			this.gameObject.SetActive(false);
		}
	}


	// Listeners


	void OnContinue ()
	{
		this.gameObject.SetActive(false);
	}


	void OnReset ()
	{/*
		Scene.Reload() ???????
		if (vehicle != null)
			{
			VPResetVehicle resetScript = vehicle.GetComponent<VPResetVehicle>();
			if (resetScript != null)
				{
				resetScript.DoReset();
				this.gameObject.SetActive(false);
				}
			}*/
	}


	void OnQuit ()
	{
		Application.Quit();
	}


	void AddListener (Button button, UnityAction method)
	{
		if (button != null) button.onClick.AddListener(method);
	}


	void RemoveListener (Button button, UnityAction method)
	{
		if (button != null) button.onClick.RemoveListener(method);
	}
}
