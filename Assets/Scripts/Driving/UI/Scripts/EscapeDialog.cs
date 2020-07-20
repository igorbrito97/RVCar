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

	void Update ()
	{
		if (Input.GetKeyDown(escapeKey))
		{
			carController.isPaused = false;
			this.gameObject.SetActive(false);
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			LevelManager.Instance.ExitSession();
		}
	}

	public void onContinue()
	{
		Debug.Log("clicou111!");
		this.gameObject.SetActive(false);
	}

	public void OnQuit()
	{
		Debug.Log("clicou2222!");
		LevelManager.Instance.ExitSession();
	}
}
