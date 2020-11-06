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
    public MainCarController manualCarController;
    public CarControllerAutomatic automaticCarController;

	public Text start;
	public Text off;
    public Color normalColor = new Color(153,153,153,255);
	public Color highlightColor = new Color(255,255,255,255);

	private bool carStarted = false;
	private bool isManual = false;

	void Start()	
	{
		manualCarController = GameObject.FindObjectOfType (typeof(MainCarController)) as MainCarController;
		if(manualCarController != null && manualCarController.enabled)
            isManual = true;
        if(!isManual)
            automaticCarController = GameObject.FindObjectOfType (typeof(CarControllerAutomatic))as CarControllerAutomatic;

		SetHighlight(start, false);
		SetHighlight(off, true);
		carStarted = false;
	}

	void Update()
	{
		bool carOn = isManual ? manualCarController.isCarOn : automaticCarController.isCarOn;
		if(carOn != carStarted)//isON
		{
			carStarted = true;
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
