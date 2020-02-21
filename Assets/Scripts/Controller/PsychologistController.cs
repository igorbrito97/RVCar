using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PsychologistController : MonoBehaviour {
    private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textPhysiotherapistID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFieldCpf;
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
    [SerializeField] private Text textCpfB;
    [SerializeField] private Text textBirthdayB;
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
        inputFieldConfirmPassword.characterLimit = 45;
        inputFieldCpf.characterLimit = 11;
        inputFieldCRP.characterLimit = 45;
        inputFieldEmail.characterLimit = 60;
        inputFieldName.characterLimit = 45;
        inputFieldPassword.characterLimit = 45; 
        inputFieldPhone.characterLimit = 15;
        inputFieldSearch.characterLimit = 45;
        inputFieldYear.characterLimit = 4;

        inputFieldCpf.contentType = InputField.ContentType.IntegerNumber;
        inputFieldEmail.contentType = InputField.ContentType.EmailAddress;
        inputFieldName.contentType = InputField.ContentType.Name;
        inputFieldPhone.contentType = InputField.ContentType.IntegerNumber;
        inputFieldYear.contentType = InputField.ContentType.IntegerNumber;
        inputFieldPassword.contentType = InputField.ContentType.Password;
        inputFieldConfirmPassword.contentType = InputField.ContentType.Password; 
        
        Begin();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Psicólogos";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
    }

    public void Clear()
    {
        textPhysiotherapistID.text = "";
        inputFieldName.text = "";
        inputFieldCpf.text = "";
        inputFieldEmail.text = "";
        inputFieldPhone.text = "";
        inputFieldYear.text = "";
        inputFieldCRP.text = "";
        inputFieldPassword.text = "";
        inputFieldConfirmPassword.text = "";
        toggleStatus.isOn = true;
        dropdownGender.value = 0;    
        dropdownDay.value = 0;
        dropdownMonth.value = 0;
        buttonDelete.gameObject.SetActive(false);
    }

    public void ShowPanel(bool flag)
    {
        if(panelEdit!=null)
            panelEdit.gameObject.SetActive(flag);

    }

    public void New()
    {
        state = 1;
        textTitle.text = "Gerenciar Psicólogos - Novo";
    }

    public void ConfirmClick()
    {
        string name = inputFieldName.text;
        string email = inputFieldEmail.text;
        string cpf = inputFieldCpf.text;
        string crp = inputFieldCRP.text;
        string phone = inputFieldPhone.text;
        string password = inputFieldPassword.text;
        string password2 = inputFieldConfirmPassword.text;
        string yearString = inputFieldYear.text;
        int day = dropdownDay.value;
        int month = dropdownMonth.value;
        int year;
        string genderString = dropdownGender.options[dropdownGender.value].text;// = dropdownGender.value;
        char gender = genderString[0];
        bool status = toggleStatus.isOn;        
        
        
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
        else if(yearString.Trim() == "")
            Debug.Log("Erro no year");
        else if(!Int32.TryParse(yearString, out year))
            Debug.Log("Erro no ano de nascimento ");
        else if(year < 1920 || year > 2010)
            Debug.Log("Erro no ano de nascimento");
        else if(password.Trim() == "")
            Debug.Log("Erro no password");
        else if(password2.Trim() == "")
            Debug.Log("Erro no password2");
        else if(password != password2)
            Debug.Log("Senhas não são iguais!");
        else {
            DateTime birthday = new DateTime(year,month+1,day+1);
            Psychologist psyc = new Psychologist(name,cpf,phone,email,gender,password,status,crp,birthday);
            if(state == 1) //adding
            {
                string returnMsg = psyc.Insert();
                Debug.Log(returnMsg);
            } 
            else if(state == 2) //alter
            {

            }
        }
    }

    public void TableLoad()
    {
        Psychologist psyc;
        List<Psychologist> psychologists;
        Button newRow;

        if (rowsClone != null)
        {
            var clones = new Transform[rows.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rows.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            row.gameObject.SetActive(true);
        }

        psyc = new Psychologist();
        psychologists = psyc.SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        Debug.Log("contagem psicologs: " + psychologists.Count);
        for(int i=0; i< psychologists.Count; i++)
        {
            textID.text = psychologists[i].Id.ToString();
            textNameB.text = psychologists[i].Name;
            textCpfB.text = psychologists[i].Cpf;
            textBirthdayB.text = psychologists[i].Birthday.ToString();
            textStatusB.text  = psychologists[i].Status ? "Ativo" : "Desativado";
        }

        rowsClone = rows;
        row.gameObject.SetActive(false);
        AddListener();

    }
    public void AddListener()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => RowClick(btn));
        }
    }

    public void RowClick(Button bt)
    {
        state = 2;
        textTitle.text = "Gerenciar Psicológos - Alterar";
    }
}
