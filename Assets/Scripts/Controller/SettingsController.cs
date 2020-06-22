using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

	[SerializeField] InputField inputFieldName;
	[SerializeField] InputField inputFieldCpf;
	[SerializeField] InputField inputFieldEmail;
	[SerializeField] InputField inputFieldPhone;
	[SerializeField] InputField inputFieldBirthday;

	//paineis 
	[SerializeField] GameObject panelUser;
	[SerializeField] GameObject panelGeneral;
	[SerializeField] GameObject gameObjSetting;
	[SerializeField] GameObject panelPsyc;
	// Use this for initialization
	void Start () {
		inputFieldName.characterLimit = 60;
		inputFieldCpf.characterLimit = 15;
		inputFieldEmail.characterLimit = 60;
		inputFieldPhone.characterLimit = 15;
		inputFieldBirthday.characterLimit = 12;

		
		inputFieldName.interactable = false;
		inputFieldCpf.interactable = false;
		inputFieldEmail.interactable = false;
		inputFieldPhone.interactable = false;
		inputFieldBirthday.interactable = false;

		Begin();
	}
    public void Begin()
    {
		panelUser.gameObject.SetActive(true);
		panelGeneral.gameObject.SetActive(false);
		LoadComponents();
    }

	private void LoadComponents()
	{
		inputFieldName.text = GameManager.instance.Psychologist.Name;
		inputFieldCpf.text = GameManager.instance.Psychologist.Cpf;
		inputFieldEmail.text = GameManager.instance.Psychologist.Email;
		inputFieldPhone.text = GameManager.instance.Psychologist.Phone;
		inputFieldBirthday.text = GameManager.instance.Psychologist.Birthday.Day + "/" + 
			GameManager.instance.Psychologist.Birthday.Month + "/" + GameManager.instance.Psychologist.Birthday.Year;
	}

	public void AlterClick()
	{
		PsychologistController controller = panelPsyc.GetComponent<PsychologistController>();
		controller.SettingsAlterClick(GameManager.instance.Psychologist);
		this.gameObject.SetActive(false);
		gameObjSetting.gameObject.SetActive(false);
	}
}
