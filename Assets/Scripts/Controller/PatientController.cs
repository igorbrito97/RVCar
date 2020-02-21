﻿using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientController : MonoBehaviour
{

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
    [SerializeField] private Text textCpfB;
    [SerializeField] private Text textBirthdayB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;


    void Start()
    {
        inputFieldCpf.characterLimit = 11;
        inputFieldEmail.characterLimit = 60;
        inputFieldName.characterLimit = 45;
        inputFieldObservation.characterLimit = 250;
        inputFieldPhone.characterLimit = 15;
        inputFieldSearch.characterLimit = 45;
        inputFieldYear.characterLimit = 4;

        inputFieldCpf.contentType = InputField.ContentType.IntegerNumber;
        inputFieldEmail.contentType = InputField.ContentType.EmailAddress;
        inputFieldName.contentType = InputField.ContentType.Name;
        inputFieldPhone.contentType = InputField.ContentType.IntegerNumber;
        inputFieldYear.contentType = InputField.ContentType.IntegerNumber;

        Begin();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Pacientes";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
    }

    public void Clear()
    {
        textPatientID.text = "";
        inputFieldName.text = "";
        inputFieldCpf.text = "";
        inputFieldEmail.text = "";
        inputFieldPhone.text = "";
        inputFieldYear.text = "";
        inputFieldObservation.text = "";
        toggleStatus.isOn = true;
        dropdownDay.value = 0;
        dropdownMonth.value = 0;
        dropdownGender.value = 0;
        buttonDelete.gameObject.SetActive(false);
    }

    public void ShowPanel(bool flag)
    {
        if (panelEdit != null)
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


        if (name.Trim() == "")
            Debug.Log("Erro no nome");
        else if (email.Trim() == "")
            Debug.Log("Erro no email");
        else if (cpf.Trim() == "")
            Debug.Log("Erro no cpf");
        else if (phone.Trim() == "")
            Debug.Log("Erro no phone");
        else if (yearString.Trim() == "")
            Debug.Log("Erro no year");
        else if (!Int32.TryParse(yearString, out year))
            Debug.Log("Erro no ano de nascimento ");
        else if (year < 1920 || year > 2010)
            Debug.Log("Erro no ano de nascimento");
        else
        {
            DateTime birthday = new DateTime(year, month + 1, day + 1);
            Patient pat = new Patient(name, cpf, birthday, phone, email, note, gender, status);
            if (state == 1)//adding
            {
                string returnMsg = pat.Insert();
                Debug.Log(returnMsg);
            }
            else if (state == 2) //alter
            {

            }
        }
    }

    public void TableLoad()
    {
        Patient pat;
        List<Patient> patients;
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

        pat = new Patient();
        try
        {
            patients = pat.SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
            Debug.Log("contagem pacientes: " + patients.Count);
            for (int i = 0; i < patients.Count; i++)
            {
                textID.text = patients[i].Id.ToString();
                textNameB.text = patients[i].Name;
                textCpfB.text = patients[i].Cpf;
                textBirthdayB.text = patients[i].Birthday.ToString();
                textStatusB.text = patients[i].Status ? "Ativo" : "Desativado";
            }
        }
        catch(Exception e)
        {
            Debug.Log("Catchando o error. - " + e);
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
        textTitle.text = "Gerenciar Paciente - Alterar";
    }
}
