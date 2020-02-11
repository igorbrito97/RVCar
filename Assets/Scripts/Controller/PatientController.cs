using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientController : MonoBehaviour {

    private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textPatientID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFieldCpf;
    [SerializeField] private InputField inputFieldEmail;
    [SerializeField] private InputField inputFieldPhone;
    [SerializeField] private Dropdown dropdownDay;
    [SerializeField] private Dropdown dropdownMonth;
    [SerializeField] private InputField inputFieldYear;
    [SerializeField] private Dropdown dropdownGender;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private InputField inputFieldObservation;
    [SerializeField] private Button buttonDelete;

    // tabela da tela inicial
    [SerializeField] private Text textID;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textRiskStratificationB;
    [SerializeField] private Text textDiseaseB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;


    void Start () {
        inputFieldCpf.characterLimit = 11;
        inputFieldEmail.characterLimit = 60;
        inputFieldName.characterLimit = 45;
        inputFieldObservation.characterLimit = 250;
        inputFieldPhone.characterLimit = 15;
        //inputFieldSearch.characterLimit = 
        inputFieldYear.characterLimit = 4;

        inputFieldCpf.contentType = InputField.ContentType.IntegerNumber;
        inputFieldEmail.contentType = InputField.ContentType.EmailAddress;
        inputFieldName.contentType = InputField.ContentType.Name;
        inputFieldPhone.contentType = InputField.ContentType.IntegerNumber;
        inputFieldYear.contentType = InputField.ContentType.IntegerNumber;

        Begin();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Pacientes";
        panelEdit.SetActive(false);
        // inputFieldSearch.text = "";
        // toggleStatusSearch.isOn = false;

    }

    public void ShowPanel(bool flag)
    {
        if(panelEdit!=null)
            panelEdit.gameObject.SetActive(flag);

    }

    public void New()
    {
        state = 1;
        textTitle.text = "Gerenciar Pacientes - Novo";
    }

    public void ConfirmClick()
    {
        string name = inputFieldName.text;
        string cpf = inputFieldCpf.text;
        string email = inputFieldEmail.text;
        string phone = inputFieldPhone.text;
        string yearString = inputFieldYear.text;
        int day = dropdownDay.value;
        int month = dropdownMonth.value;
        int year;
        string genderString = dropdownGender.options[dropdownGender.value].text;// = dropdownGender.value;
        char gender = genderString[0];
        bool status = toggleStatus.isOn;    
        string note = inputFieldObservation.text;    
        
        
        if(name.Trim() == "")
            Debug.Log("Erro no nome");
        else if(email.Trim() == "")
            Debug.Log("Erro no email");
        else if(cpf.Trim() == "")
            Debug.Log("Erro no cpf");
        else if(phone.Trim() == "")
            Debug.Log("Erro no phone");
        else if(yearString.Trim() == "")
            Debug.Log("Erro no year");
        else if(!Int32.TryParse(yearString, out year))
            Debug.Log("Erro no ano de nascimento ");
        else if(year < 1920 || year > 2010)
            Debug.Log("Erro no ano de nascimento");
        else {
            DateTime birthday = new DateTime(year,month+1,day+1);
            Patient pat = new Patient(name,cpf,birthday,phone,email,note,gender,status);
            if(state == 1)//adding
            {
                string returnMsg = pat.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2) //alter
            {

            }
        }
    }
}
