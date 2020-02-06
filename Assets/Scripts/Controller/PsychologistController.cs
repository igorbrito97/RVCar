using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PsychologistController : MonoBehaviour {

    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textPhysiotherapistID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFielCpf;
    [SerializeField] private InputField inputFieldEmail;
    [SerializeField] private InputField inputFieldPhone;
    [SerializeField] private Dropdown dropdownDay;
    [SerializeField] private Dropdown dropdownMonth;
    [SerializeField] private InputField inputFieldYear;
    [SerializeField] private Dropdown dropdownGender;    
    [SerializeField] private InputField inputFieldCRP;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private InputField inputFieldPassword;
    [SerializeField] private InputField inputFieldConfirmPassword;
    [SerializeField] private Button buttonDelete;

    // tabela da tela inicial
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textCrefitoB;
    [SerializeField] private Text textUserProfileB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Text textID;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;
    
    // mensagem de erro 
    [SerializeField] private Button buttonMessage;

    void Start () {
        inputFieldPassword.inputType = InputField.InputType.Password;
        inputFieldConfirmPassword.inputType = InputField.InputType.Password;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FirstClick()
    {

    }

    public void ShowPanel(bool flag)
    {
        if(panelEdit!=null)
            panelEdit.gameObject.SetActive(flag);

    }

    public void InsertPsyc()
    {
        string name = inputFieldName.text;
        string email = inputFieldEmail.text;
        string cpf = inputFielCpf.text;
        string crp = inputFieldCRP.text;
        string phone = inputFieldPhone.text;
        string password = inputFieldPassword.text;
        string password2 = inputFieldConfirmPassword.text;
        string year = inputFieldYear.text;
        int day = dropdownDay.value;
        int month = dropdownMonth.value;
        string genderS = dropdownGender.options[dropdownGender.value].text;// = dropdownGender.value;
        char gender = genderS[0];
        bool status;

        Debug.Log("dia " + day + " mes " + month + " sexu " + gender);
        //DateTime birthday = new DateTime(year,month,day); transformar o year para int
        
        if(name.Trim() == "")
            Debug.Log("Erro no nome");
        else if(email.Trim() == "")
            Debug.Log("Erro no email");
        else if(cpf.Trim() == "")
            Debug.Log("Erro no cpf");
        else if(crp.Trim() == "")
            Debug.Log("Erro no crp");
        else if(phone.Trim() == "")
            Debug.Log("Erro no phone");
        else if(year.Trim() == "")
            Debug.Log("Erro no year");
        else if(password.Trim() == "")
            Debug.Log("Erro no password");
        else if(password2.Trim() == "")
            Debug.Log("Erro no password2");
        else if(password != password2)
            Debug.Log("Senhas não são iguais!");
        else {
            Debug.Log("infos" + "nome - " + name + 
            " email - " + email +
            " cpf - " + cpf + 
            " crp - " + crp + 
            " phone - " + phone + 
            " password - " + password + " password2 - " + password2);

        }
    }
}
