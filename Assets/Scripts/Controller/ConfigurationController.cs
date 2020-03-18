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
    //tabela para componente
    [SerializeField] private Dropdown dropdownScenario;
    [SerializeField] private Button deleteScenarioButton;
    [SerializeField] private Text textIDBScenario;
    [SerializeField] private Text textNameBScenario;
    [SerializeField] private Button rowScenario;
    [SerializeField] private GameObject rows2;
    //private List<Configuration> scenarioList;
    private KeyValuePair<int,string>[] arrayAllScenarios;
    private List<KeyValuePair<int,string>> listScenario = new List<KeyValuePair<int, string>>();
    private int selectedScenarioIndex;

    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textDescriptionB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelTableComponent;

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
        panelTableComponent.SetActive(false);
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
        listScenario.Clear();
        selectedScenarioIndex = -1;
        //tableScenarioClear
        var clones = new Transform[rows2.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rows2.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
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
        {
            textTitle.text = "Gerenciar Componentes - Novo";
            panelTableComponent.SetActive(true);
            LoadDropdown();
            //scenarioList = new List<Configuration>();
        }
    }

    public void LoadDropdown()
    {
        Configuration scenario = new Configuration(1);
        List<Configuration> lista = scenario.SearchAll("",false);
        dropdownScenario.ClearOptions();
        arrayAllScenarios = new KeyValuePair<int, string>[lista.Count];
        for(int i = 0; i < lista.Count; i++)
        {
            arrayAllScenarios[i] = new KeyValuePair<int, string>(lista[i].Id,lista[i].Name);
            dropdownScenario.options.Add(new Dropdown.OptionData(lista[i].Name));
        }
        dropdownScenario.value = -1;
        dropdownScenario.value = 0;
    }

    public void ConfirmClick()
    {
        option = dropdownConfigOption.value;
        string id = textConfigID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        Configuration config;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else if(option == 2 && listScenario.Count == 0)
            Debug.Log("Erro nos cenários. É necessário pelo menos 1!");
        else 
        {
            if(option == 2) //component
                config = new Configuration(option,name,description,status ? 1 : 0,listScenario);
            else //others
                config = new Configuration(option,name,description,status ? 1 : 0);
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
        List<Button> btns = new List<Button>();
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
        AddListener(btns,option);

    }
    public void AddListener(List<Button> btns,int opt)
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => RowClick(btn,opt));
        }
    }

    public void RowClick(Button br,int opt)
    {
        state = 2;
        Configuration config = new Configuration(opt).Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);

        textConfigID.text = config.Id.ToString();
        inputFieldName.text = config.Name;
        inputFieldDescription.text = config.Description;
        
        toggleStatus.isOn = config.Status == 1;

        if(opt == 0)
            textTitle.text = "Gerenciar Climas - Alterar";
        else if(opt == 1)
            textTitle.text = "Gerenciar Cenários - Alterar";
        else 
        {
            textTitle.text = "Gerenciar Componentes - Alterar";
            panelTableComponent.SetActive(true);
            LoadDropdown();
            Debug.Log("cenariosCount " + config.ListScenarios.Count);
            listScenario = config.ListScenarios;
            foreach(KeyValuePair<int,string> par in listScenario)
            {
                AddRowTableScenario(par);
            }
        }

        
    }

    public void AddScenarioClick()
    {
        if(listScenario.Contains(arrayAllScenarios[dropdownScenario.value])){
            Debug.Log("ja existe na lista! Nao add");
            return;
        }
        else {
            listScenario.Add(arrayAllScenarios[dropdownScenario.value]);
            AddRowTableScenario(arrayAllScenarios[dropdownScenario.value]);
        }
    }

    private void AddRowTableScenario(KeyValuePair<int,string> info)
    {
        Button newRow;
        rowScenario.gameObject.SetActive(true);
        textIDBScenario.text = info.Key.ToString();
        textNameBScenario.text = info.Value;
        newRow = Instantiate(rowScenario) as Button; 
        newRow.transform.SetParent(rowScenario.transform.parent,false);
        newRow.onClick.AddListener(() => RowClick2(newRow));
        rowScenario.gameObject.SetActive(false);
    }

    public void RowClick2(Button br)
    {
        //FAZER: QUANDO CLICAR DEIXAR A LINHA DE COR DIFERENTE, SE CLICAR NOVAMENTE TIRA
        int selectedScenarioKey = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDBScenario).text);
        int i=0;
        foreach(KeyValuePair<int,string> par in listScenario)
        {
            if(par.Key == selectedScenarioKey)
            {
                selectedScenarioIndex = i;
                break;
            }
            i++;
        }
    }

    public void DeleteScenarioClick()
    {
        if(selectedScenarioIndex < 0)
            Debug.Log("Erro ao excluir!");
        else {
            var clone = rows2.transform.GetChild(selectedScenarioIndex+1); //+1 porque o primeiro ta false
            Destroy(clone.gameObject);
            listScenario.RemoveAt(selectedScenarioIndex);
            selectedScenarioIndex = -1;
        }
    }
}
