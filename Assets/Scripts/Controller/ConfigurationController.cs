using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationController : MonoBehaviour
{
    private int state; //0 - nada, 1 - novo, 2 - alterando
    private int option; //0 - weather, 1 - scenario, 2 - component
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private Dropdown dropdownConfigOption;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textConfigID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Toggle toggleStatus;  
    [SerializeField] private Button buttonDelete;

    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textDescriptionB;
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
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;

        Begin();
    }

    void Update()
    {
        
    }

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Configuraçoes da Sessao";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        toggleStatus.isOn = true;
        buttonDelete.gameObject.SetActive(false);
    }

    public void New()
    {
        state = 1;
        panelEdit.gameObject.SetActive(true);
        option = dropdownConfigOption.value;
        if(option == 0)
            textTitle.text = "Gerenciar Climas - Novo";
        else if(option == 1)
            textTitle.text = "Gerenciar Cenários - Novo";
        else 
            textTitle.text = "Gerenciar Componentes - Novo";
    }

    public void ConfirmClick()
    {
        option = dropdownConfigOption.value;
        string id = textConfigID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else 
        {
            Configuration config = new Configuration(option,name,description,status ? 1 : 0);
            if(state == 1) //add
            {
                string returnMsg = config.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)//alter
            {
                string returnMsg = config.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
            }
        }
    }

    public void DeleteClick()
    {
        option = dropdownConfigOption.value;
        string id = textConfigID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            Debug.Log("Configuração ja desativada!");
            return;
        }

        if(new Configuration(option).Delete(Convert.ToInt32(id)))
        {
            Begin();
            Debug.Log("Sucesso!");
        }
        else
        {
            Debug.Log("Erro ao deletar!");
        }
    }

    public void TableLoad()
    {
        option = dropdownConfigOption.value;
        Configuration config;
        List<Configuration> configurations;
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

        config = new Configuration(option);
        configurations = config.SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        for(int i = 0; i < configurations.Count; i++)
        {
            textIDB.text = configurations[i].Id.ToString();
            textNameB.text = configurations[i].Name;
            textDescriptionB.text = configurations[i].Description;
            textStatusB.text = configurations[i].Status == 1 ? "Ativo" : "Desativado";
            newRow = Instantiate(row) as Button; 
            newRow.transform.SetParent(row.transform.parent,false);
            btns.Add(newRow);
        }

        rowsClone = rows;
        row.gameObject.SetActive(false);
        AddListener(option);

    }
    public void AddListener(int opt)
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => RowClick(btn,opt));
        }
    }

    public void RowClick(Button br,int opt)
    {
        state = 2;
        if(opt == 0)
            textTitle.text = "Gerenciar Climas - Alterar";
        else if(opt == 1)
            textTitle.text = "Gerenciar Cenários - Alterar";
        else 
            textTitle.text = "Gerenciar Componentes - Alterar";

        Configuration config = new Configuration(opt).Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);

        textConfigID.text = config.Id.ToString();
        inputFieldName.text = config.Name;
        inputFieldDescription.text = config.Description;
        
        toggleStatus.isOn = config.Status == 1;
    }
}
