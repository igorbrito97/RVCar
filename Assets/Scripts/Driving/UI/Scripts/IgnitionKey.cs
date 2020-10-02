//--------------------------------------------------------------
//      Vehicle Physics Pro: advanced vehicle physics kit
//          Copyright © 2011-2019 Angel Garcia "Edy"
//        http://vehiclephysics.com | @VehiclePhysics
//--------------------------------------------------------------

// IgnitionKey: UI for the ignition key


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IgnitionKey : MonoBehaviour
{
	public MainCarController carController;

	public Text start;
	public Text off;
    public Color normalColor = new Color(153,153,153,255);
	public Color highlightColor = new Color(255,255,255,255);

	private bool carStarted;


	void Start()	
	{
		carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
		SetHighlight(start, false);
		SetHighlight(off, true);
		carStarted = false;
	}
	void OnEnable ()
		{
		//if (vehicle != null)
			//vehicle.onBeforeIntegrationStep += UpdateKeyInput;
		}


	void OnDisable ()
		{
		//if (vehicle != null)
			//vehicle.onBeforeIntegrationStep -= UpdateKeyInput;
		}

	void Update()
	{
		if(carController.isCarOn != carStarted)//isON
		{
			carStarted = carController.isCarOn;
			SetHighlight(start, true);
			SetHighlight(off, false);
		}
	}


	void SetHighlight (Text text, bool highlight)
		{
		if (text != null)
			text.color = highlight? highlightColor : normalColor;
		}

}
