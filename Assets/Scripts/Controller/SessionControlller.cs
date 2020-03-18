using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionControlller : MonoBehaviour 
{
    private int state = 0; // 0 nada, 1 - add, 2 - alter
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private Toggle toggleStatusSearch;
    [SerializeField] private InputField inputFieldSearch;

    // campos da tela de edição
    [SerializeField] private Text textSessionID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Dropdown dropdownScenario;
    [SerializeField] private Dropdown dropdownWeather;
    [SerializeField] private Slider sliderTime;
    [SerializeField] private Text HourText;
    [SerializeField] private Button buttonAdd;

    //tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textScenarioB;
    [SerializeField] private Text textWeatherB;
    [SerializeField] private Text textTimeB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button buttonDelete;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

    //tabela para componente
    [SerializeField] private Dropdown dropdownComponents;
    [SerializeField] private Button deleteComponentButton;
    [SerializeField] private Text textIDBComponent;
    [SerializeField] private Text textNameBComponent;
    [SerializeField] private Text textInfoBComponent;
    [SerializeField] private Button rowComponent;
    [SerializeField] private GameObject rows2;

    //lista de objetos
    private List<Button> btns = new List<Button>();
    private KeyValuePair<int,string>[] arrayAllScenarios;
    private KeyValuePair<int,string>[] arrayAllWeathers;
    private KeyValuePair<int,string>[] arrayAllComponents;
    private List<KeyValuePair<int,string>> listComponents = new List<KeyValuePair<int, string>>();
    private int selectedComponentIndex;
    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;

    // // Tabela de movimentos da fase
    // private GameObject rowsClone2 = null;
    // private List<Button> btns2 = new List<Button>();
    // [SerializeField] private Button row2;
    // [SerializeField] GameObject rows2;
    // [SerializeField] private Text textPosB2;
    // [SerializeField] private Text textMovementB2;
    // [SerializeField] private Text textCategoryB2;
    // [SerializeField] private Text textDurationB2;

    // //
    // [SerializeField] private GameObject panelAlterAdd;

    // // AddMovementPanel
    // [SerializeField] private GameObject panelStageMovementAdd;
    // [SerializeField] private InputField inputFieldSearchAdd; // sm = stage movement
    // [SerializeField] private InputField inputFieldNameSMPAdd;
    // [SerializeField] private InputField inputFieldCategorySMPAdd;
    // [SerializeField] private InputField inputFieldDurationSMPAdd;
    // [SerializeField] private InputField inputFieldRestingSMPAdd;
    // [SerializeField] private Slider sliderIntensitySMPAdd;
    // [SerializeField] private Text textTitleAlterAdd;
    // [SerializeField] private Text textIDSMADD;
    // [SerializeField] private Text textMovementSMADD;
    // [SerializeField] private Text textCategorySMADD;
    // [SerializeField] private Button rowAdd;
    // [SerializeField] GameObject rowsAdd;
    // private GameObject rowsCloneAdd = null;
    // private List<Button> btnsAdd = new List<Button>();

    // // AlterMovementsPanel
    // [SerializeField] private GameObject panelStageMovementAlter;
    // [SerializeField] private InputField inputFieldNameSMPAlter;
    // [SerializeField] private InputField inputFieldCategorySMPAlter;
    // [SerializeField] private InputField inputFieldDurationSMPAlter;
    // [SerializeField] private InputField inputFieldRestingSMPAlter;
    // [SerializeField] private Slider sliderIntensitySMPAlter;


    void Start () {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;

		Begin();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Sessão";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        sliderTime.value = 12;
        Clear();
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        buttonDelete.gameObject.SetActive(false);
        ClearTableComponent();        
    }

    private void ClearTableComponent()
    {
        listComponents.Clear();
        selectedComponentIndex = -1;
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
        textTitle.text = "Gerenciar Sessão - Novo";
        panelEdit.gameObject.SetActive(true);
        Clear();
        LoadDropdowns();
    }

    public void LoadDropdowns() //scenario and weather
    {
        Configuration weather = new Configuration(0);
        dropdownWeather.ClearOptions();
        List<Configuration> weatherList = weather.SearchAll("",false);
        arrayAllWeathers = new KeyValuePair<int, string>[weatherList.Count];
        for(int i = 0; i < weatherList.Count; i++)
        {
            arrayAllWeathers[i] = new KeyValuePair<int, string>(weatherList[i].Id,weatherList[i].Name);
            dropdownWeather.options.Add(new Dropdown.OptionData(weatherList[i].Name));
        }
        dropdownWeather.value = -1;
        dropdownWeather.value = 0;

        Configuration scenario = new Configuration(1);
        dropdownScenario.ClearOptions();
        List<Configuration> scenarioList = scenario.SearchAll("",false);
        arrayAllScenarios = new KeyValuePair<int, string>[scenarioList.Count];
        for(int i = 0; i < scenarioList.Count; i++)
        {
            arrayAllScenarios[i] = new KeyValuePair<int, string>(scenarioList[i].Id,scenarioList[i].Name);
            dropdownScenario.options.Add(new Dropdown.OptionData(scenarioList[i].Name));
        }
        dropdownScenario.value = -1;
        dropdownScenario.value = 0;

    }

    public void OnChangeTimeSlider()
    {
        float val = sliderTime.value;
        HourText.text = val + " h";
    }

    public void TableLoad()
    {
        List<Session> sessoes;
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

        List<Button> bts = new List<Button>();
        sessoes = new Session().SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        for(int i = 0; i< sessoes.Count; i++)
        {
            textIDB.text = sessoes[i].Id.ToString();
            textNameB.text = sessoes[i].Name;
            textScenarioB.text = sessoes[i].Scenario.Name;
            textWeatherB.text = sessoes[i].Weather.Name;
            textTimeB.text = sessoes[i].Time.ToString() + " h";
            textStatusB.text = sessoes[i].Status == 1 ? "Ativo" : "Desativado";
            newRow = Instantiate(row) as Button;
            newRow.transform.SetParent(row.transform.parent,false);
            bts.Add(newRow);
        }
        rowsClone = rows;
        row.gameObject.SetActive(false);
        AddListener(bts);

    }
    public void AddListener(List<Button> bts)
    {
        foreach (Button btn in bts)
        {
            btn.onClick.AddListener(() => RowClick(btn));
        }
    }

    public void RowClick(Button br)
    {
        state = 2;
        Session session = new Session().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);
        textTitle.text = "Gerenciar Sessão - Alterar";
        Debug.Log("time: " + session.Time + " qntdCompo: " + session.Components.Count);

        textSessionID.text = session.Id.ToString();
        inputFieldName.text = session.Name;
        inputFieldDescription.text = session.Description;
        toggleStatus.isOn = session.Status == 1;
        sliderTime.value = session.Time;
        //se ele escolher algum que agora esta desativado = da mensagem de erro e coloca o 1o dropdown
        LoadDropdowns();
        if(session.Scenario.Status == 1)
        {
            int i=0;
            while(i < dropdownScenario.options.Count && 
                arrayAllScenarios[i].Key != session.Scenario.Id)
                i++;
            dropdownScenario.value = i;
        }
        else
            Debug.Log("Cenário da sessão esta atualmente desativado!");
        
        if(session.Weather.Status == 1)
        {
            int i=0;
            while(i < dropdownWeather.options.Count && 
                arrayAllWeathers[i].Key != session.Weather.Id)
                i++;
            dropdownWeather.value = i;
        }
        else
            Debug.Log("Clima da sessão esta atualmente desativado!");

        foreach(Configuration config in session.Components)
        {
            listComponents.Add(new KeyValuePair<int, string>(config.Id,config.Name));
            AddTableComponentRow(new KeyValuePair<int, string>(config.Id,config.Name));
        }        
    }

    public void ConfirmClick()
    {
        string id = textSessionID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        int scenario_id,weather_id;
        int time = (int) sliderTime.value;
        bool status = toggleStatus.isOn;
        Session session;


        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else if(dropdownScenario.value < 0)
            Debug.Log("Erro ao escolher cenário!");
        else if(dropdownWeather.value < 0)
            Debug.Log("Erro ao escolher clima!");
        else if(time < 0 || time > 23)
            Debug.Log("Erro ao escolher hora!");
        else 
        {// criei uma config com option e id, usar essas para inserir e alterar. Para buscar pega com todas as infos
            //setar as classes da session
            scenario_id = arrayAllScenarios[dropdownScenario.value].Key;
            weather_id = arrayAllWeathers[dropdownWeather.value].Key;
            List<Configuration> list = new List<Configuration>();
            foreach(KeyValuePair<int,string> par in listComponents)
            {
                list.Add(new Configuration(par.Key,2));
            }
            session = new Session(name,description,new Configuration(weather_id,0),time,new Configuration(scenario_id,1),list,status ? 1 : 0);
            if(state == 1) //add
            {
                string returnMsg = session.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)//alter
            {
                string returnMsg = session.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
            }
        }
    }

    public void DeleteClick()
    {
        string id = textSessionID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            Debug.Log("Sessão ja desativada!");
            return;
        }

        if(new Session().Delete(Convert.ToInt32(id)))
        {
            Begin();
            Debug.Log("Sucesso!");
        }
        else
        {
            Debug.Log("Erro ao deletar!");
        }
    }
    public void OnChangeScenarioDropdown() //load components
    {
        ClearTableComponent();
        dropdownComponents.ClearOptions();
        List<Configuration> componentsList = new Configuration(2).GetComponentsByScenario(arrayAllScenarios[dropdownScenario.value].Key);
        arrayAllComponents = new KeyValuePair<int, string>[componentsList.Count];
        for(int i = 0; i< componentsList.Count; i++)
        {
            arrayAllComponents[i] = new KeyValuePair<int, string>(componentsList[i].Id,componentsList[i].Name);
            dropdownComponents.options.Add(new Dropdown.OptionData(componentsList[i].Name));
        }
        dropdownComponents.value = -1;
        dropdownComponents.value = 0;
    }

    public void AddComponentClick() 
    {
        if(listComponents.Contains(arrayAllComponents[dropdownComponents.value])) {
            Debug.Log("ja esta na lista. Não add!");
            return;
        }
        else {
            listComponents.Add(arrayAllComponents[dropdownComponents.value]);
            AddTableComponentRow(arrayAllComponents[dropdownComponents.value]);
        }
    }

    private void AddTableComponentRow(KeyValuePair<int,string> info)
    {
        Button newRow;
        rowComponent.gameObject.SetActive(true);
        textIDBComponent.text = info.Key.ToString();
        textNameBComponent.text = info.Value;
        //textInfoBComponent.text =  por enquanto sem info
        newRow = Instantiate(rowComponent) as Button; 
        newRow.transform.SetParent(rowComponent.transform.parent,false);
        newRow.onClick.AddListener(() => RowClick2(newRow));
        rowComponent.gameObject.SetActive(false);
    }

    private void RowClick2(Button br)
    {
        int selectedKey = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDBComponent).text);
        int i=0;
        foreach(KeyValuePair<int,string> par in listComponents)
        {
            if(par.Key == selectedKey)
            {
                selectedComponentIndex = i;
                break;
            }
            i++;
        }
    }

    public void DeleteComponentClick()
    {
         if(selectedComponentIndex < 0)
            Debug.Log("Erro ao excluir!");
        else {
            var clone = rows2.transform.GetChild(selectedComponentIndex+1); //+1 porque o primeiro ta false
            Destroy(clone.gameObject);
            listComponents.RemoveAt(selectedComponentIndex);
            selectedComponentIndex = -1;
        }
    }
}
