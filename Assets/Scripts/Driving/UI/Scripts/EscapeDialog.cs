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
	[SerializeField] private Text ConfirmText;
	[SerializeField] private GameObject AreYouSurePanel;
	

	void Start()
	{
		carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
	}	

	void Update ()
	{
		//https://answers.unity.com/questions/174448/stop-physics-without-using-timetimescale-0.html
		//salvar a velocity e voltar depois - verificar se tem componente que se movimenta, se tiver faz deles tbm (antes acho que tem que salvar sessão - ai ja usa para reiniciar)
		// nao vai dar nao -> ver de colocar o Time.deltaTime no CarEngine Direção - quanto tem que multiplicar pra ficar bom (*descobrir*)
		if (Input.GetKeyDown(escapeKey))
		{
			if(carController.isPaused)
				OnContinue();
			else {
				carController.isPaused = true;
				//carVelocity = carController.CarRigidbody.velocity;
				//carController.CarRigidbody.Sleep();
				//carController.CarRigidbody.isKinematic = true;
				Time.timeScale = 0;
				this.gameObject.SetActive(true);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Return))
			OnQuit();
		else if(Input.GetKeyDown(KeyCode.Space))
			OnRestart();
	}

	public void OnContinue()
	{
		//carController.CarRigidbody.velocity = carVelocity;
		//carController.CarRigidbody.isKinematic = false;
		//carController.CarRigidbody.WakeUp();
		carController.isPaused = false;
		this.gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	public void OnRestart()
	{
		ConfirmText.text = "Deseja realmente reiniciar a sessão?";
		AreYouSurePanel.GetComponent<ConfirmSessionAction>().Action = 0;
		AreYouSurePanel.gameObject.SetActive(true);
		this.gameObject.SetActive(false);
	}

	public void OnQuit()
	{
		ConfirmText.text = "Deseja realmente encerrar a sessão?";
		AreYouSurePanel.GetComponent<ConfirmSessionAction>().Action = 1;
		AreYouSurePanel.gameObject.SetActive(true);
		this.gameObject.SetActive(false);
	}
}
