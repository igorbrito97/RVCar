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
    [SerializeField] private Text textPsychologistID;
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
        textPsychologistID.text = "";
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

    public void New()
    {
        state = 1;
        textTitle.text = "Gerenciar Psicólogos - Novo";
        panelEdit.gameObject.SetActive(true);
    }

    public void ConfirmClick()
    {
        string id = textPsychologistID.text;
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
            LevelManager.Instance.AlterMessage("Nome inválido. Digite um nome válido!",Color.red);
        else if(email.Trim() == "")
            LevelManager.Instance.AlterMessage("Email inválido. Digite um email válido!",Color.red);
        else if(cpf.Trim() == "")
            LevelManager.Instance.AlterMessage("CPF inválido. Digite um CPF válido!",Color.red);
        else if(crp.Trim() == "")
            LevelManager.Instance.AlterMessage("CRP inválido. Digite um CRP válido!",Color.red);
        else if(phone.Trim() == "")
            LevelManager.Instance.AlterMessage("Telefone inválido. Digite um telefone válido!",Color.red);
        else if(yearString.Trim() == "")
            LevelManager.Instance.AlterMessage("Ano de nascimento inválido. Digite um ano válido!",Color.red);
        else if(!Int32.TryParse(yearString, out year))
            LevelManager.Instance.AlterMessage("Ano de nascimento inválido. Digite um ano válido!",Color.red);
        else if(year < 1920 || year > 2010)
            LevelManager.Instance.AlterMessage("Ano de nascimento inválido. Digite um ano válido! (É necessário ter 18 anos)",Color.red);
        else if(password.Trim() == "")
            LevelManager.Instance.AlterMessage("Senha inválida. Digite uma senha válida!",Color.red);
        else if(password2.Trim() == "")
            LevelManager.Instance.AlterMessage("Senha inválida. Digite uma senha válida!",Color.red);
        else if(password != password2)
            LevelManager.Instance.AlterMessage("Senhas diferentes. Digite novamente, as duas senhas devem ser iguais",Color.red);
        else {
            DateTime birthday = new DateTime(year,month+1,day+1);
            Psychologist psyc = new Psychologist(name,cpf,phone,email,gender,password,status ? 1 : 0,crp,birthday);
            if(state == 1) //adding
            {
                string returnMsg = psyc.Insert();
                if(returnMsg.Equals("Ok"))
                {
                    LevelManager.Instance.AlterMessage("Psicólogo inserido com sucesso!",Color.red);
                    Begin();
                    if(rowsClone!=null)
                        ClearMainTable();
                }
                else
                {
                    LevelManager.Instance.AlterMessage(returnMsg,Color.red);
                }
            } 
            else if(state == 2) //alter
            {
                string returnMsg = psyc.Alter(Convert.ToInt32(id));
                if(returnMsg.Equals("Ok"))
                {
                    LevelManager.Instance.AlterMessage("Psicólogo alterado com sucesso!",Color.red);
                    Begin();
                    if(rowsClone!=null)
                        ClearMainTable();
                }
                else
                {
                    LevelManager.Instance.AlterMessage(returnMsg,Color.red);
                }
            }
        }
    }

    public void DeleteClick()
    {
        string id = textPsychologistID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            LevelManager.Instance.AlterMessage("Erro. Psicólogo já desativado!",Color.red);
            return;
        }

        if(new Psychologist().Delete(Convert.ToInt32(id)))
        {
            Begin();
            LevelManager.Instance.AlterMessage("Psicólogo desativado com sucesso!",Color.green);
        }
        else {
            LevelManager.Instance.AlterMessage("Erro ao desativar Psicólogo!",Color.red);
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


        List<Button> btns = new List<Button>();
        psyc = new Psychologist();
        psychologists = psyc.SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        Debug.Log("contagem psicologs: " + psychologists.Count);
        for(int i=0; i< psychologists.Count; i++)
        {
            textID.text = psychologists[i].Id.ToString();
            textNameB.text = psychologists[i].Name;
            textCpfB.text = psychologists[i].Cpf;
            textBirthdayB.text = psychologists[i].Birthday.ToString();
            textStatusB.text  = psychologists[i].Status == 1 ? "Ativo" : "Desativado";
            newRow = Instantiate(row) as Button; 
            newRow.transform.SetParent(row.transform.parent,false);
            btns.Add(newRow);
        }

        rowsClone = rows;
        row.gameObject.SetActive(false);
        AddListener(btns);

    }
    public void AddListener(List<Button> btns)
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => RowClick(btn));
        }
    }

    public void RowClick(Button br)
    {
        Psychologist psyc = new Psychologist().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textID).text));
        LoadPage(psyc);
    }

    private void LoadPage(Psychologist psychologist)
    {
        state = 2;
        textTitle.text = "Gerenciar Psicológos - Alterar";
        buttonDelete.gameObject.SetActive(true);
        textPsychologistID.text = psychologist.Id.ToString();
        inputFieldName.text = psychologist.Name;
        inputFieldCpf.text = psychologist.Cpf;
        inputFieldEmail.text = psychologist.Email;
        inputFieldPhone.text = psychologist.Phone;
        inputFieldYear.text = psychologist.Birthday.Year + "";
        inputFieldCRP.text = psychologist.Crp;
        inputFieldPassword.text = inputFieldConfirmPassword.text = psychologist.Password;

        toggleStatus.isOn = psychologist.Status == 1;

        dropdownDay.value = psychologist.Birthday.Day -1;
        dropdownMonth.value = psychologist.Birthday.Month-1;
        int i=0;
        while(i < dropdownGender.options.Count && 
            psychologist.Gender != dropdownGender.options[i].text[0])
            i++;
        dropdownGender.value = i;  
    }

    public void SettingsAlterClick(Psychologist psyc)
    {
        this.gameObject.SetActive(true);
        panelEdit.gameObject.SetActive(true);
        LoadPage(psyc);
    }
    public void ClearMainTable()
    {
        var clones = new Transform[rows.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rows.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
        textID.text = textNameB.text = textCpfB.text = textBirthdayB.text = textStatusB.text = "";
        row.gameObject.SetActive(true);
        rowsClone = null;
    }
    void onDisable()
    {
        if(rowsClone != null)
            ClearMainTable();
        Begin();
    }
}
