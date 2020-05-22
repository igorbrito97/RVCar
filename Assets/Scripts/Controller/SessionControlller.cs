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
    [SerializeField] private InputField inputFieldPsychologist;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private Toggle toggleIsPublic;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private InputField inputFieldPatient;
    [SerializeField] private InputField inputFieldScenario;
    [SerializeField] private InputField inputFieldWeather;
    [SerializeField] private Button buttonSelectPatient;
    [SerializeField] private Button buttonSelectScenario;
    [SerializeField] private Button buttonSelectWeather;
    [SerializeField] private Button buttonAddComponent;
    [SerializeField] private Button buttonDeleteComponent;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private Button buttonAlter;
    [SerializeField] private Button buttonDuplicate;
    [SerializeField] private Button buttonDelete;

    //tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textPsychologistB;
    [SerializeField] private Text textPublicB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;   

    //tabela para componente (panelEdit)
    [SerializeField] private Dropdown dropdownComponent;
    [SerializeField] private Text textComponentIDB;
    [SerializeField] private Text textComponentNameB;
    [SerializeField] private Button rowComponent;
    [SerializeField] private GameObject rowsComp;
    private KeyValuePair<int,string>[] arrayAllComponents;
    private List<KeyValuePair<int,string>> listComponent = new List<KeyValuePair<int, string>>();
    private int selectedComponentIndex;

    //tabela de pacientes
    private int selectedPatId = 0;
    [SerializeField] private InputField inputFieldPatSearch;
    [SerializeField] private Text textPatIDB;
    [SerializeField] private Text textPatNameB;
    [SerializeField] private Text textPatCpfB;
    [SerializeField] private Text textPatBirthdayB;
    [SerializeField] private Button rowPat;
    [SerializeField] GameObject rowsPatient;
    private GameObject rowsClonePat = null;

    //campos tela de paciente
    [SerializeField] private InputField inputFieldPatName;
    [SerializeField] private InputField inputFieldPatCpf;
    [SerializeField] private InputField inputFieldPatPhone;
    [SerializeField] private InputField inputFieldPatNote;
    [SerializeField] private InputField inputFieldPatEmail;
    [SerializeField] private InputField inputFieldPatGender;
    [SerializeField] private InputField inputFieldPatBirthday;

    //tabela de cenario
    private int selectedScenarioId = 0;
    [SerializeField] private InputField inputFieldScenarioSearch;
    [SerializeField] private Text textScenarioIDB;
    [SerializeField] private Text textScenarioNameB;
    [SerializeField] private Text textScenarioEnvTypeB;
    [SerializeField] private Button rowSce;
    [SerializeField] GameObject rowsScenario;
    private GameObject rowsCloneSce = null;

    //campos tela de fases
    [SerializeField] private InputField inputFieldScenarioName;
    [SerializeField] private InputField inputFieldScenarioEnvType;
    [SerializeField] private InputField inputFieldScenarioDescription;

    //tabela de clima
    private int selectedWeatherId = 0;
    [SerializeField] private InputField inputFieldWeatherSearch;
    [SerializeField] private Text textWeatherIDB;
    [SerializeField] private Text textWeatherNameB;
    [SerializeField] private Text textWeatherTypeB;
    [SerializeField] private Button rowWea;
    [SerializeField] GameObject rowsWeather;
    private GameObject rowsCloneWea = null;

    //campos tela de fases
    [SerializeField] private InputField inputFieldWeatherName;
    [SerializeField] private InputField inputFieldWeatherType;
    [SerializeField] private InputField inputFieldWeatherDescription;

    //lista de objetos
    private Scenario currentScenario = null;
    private Patient currentPatient = null;
    private Weather currentWeather = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelSelectScenario;
    [SerializeField] private GameObject panelScenario1;
    [SerializeField] private GameObject panelScenario2;
    [SerializeField] private GameObject panelSelectPatient;
    [SerializeField] private GameObject panelPatient1;
    [SerializeField] private GameObject panelPatient2;
    [SerializeField] private GameObject panelPatient3;
    [SerializeField] private GameObject panelSelectWeather;
    [SerializeField] private GameObject panelWeather1;
    [SerializeField] private GameObject panelWeather2;

    void Start () {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 450;
        inputFieldPsychologist.interactable = false;
        inputFieldPatSearch.characterLimit = 60;
        inputFieldScenarioSearch.characterLimit = 60;
        inputFieldWeatherSearch.characterLimit = 60;

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
        panelSelectPatient.gameObject.SetActive(false);
        panelSelectScenario.gameObject.SetActive(false);
        panelSelectWeather.gameObject.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
        changePanelEditInteractable(false);
        buttonConfirm.gameObject.SetActive(true);
        buttonAlter.gameObject.SetActive(true);
        buttonDelete.gameObject.SetActive(true);
        buttonDuplicate.gameObject.SetActive(true);
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        currentPatient = new Patient();
        selectedPatId = 0;
        currentScenario = new Scenario();
        selectedScenarioId = 0;
        currentWeather = new Weather();
        selectedWeatherId = 0;
        inputFieldPatient.text = "";
        inputFieldScenario.text = "";
        inputFieldWeather.text = "";
        ClearComponentTable();
    }

    public void New()
    {
        state = 1;
        textTitle.text = "Gerenciar Sessão - Novo";
        panelEdit.gameObject.SetActive(true);
        Clear();
        changePanelEditInteractable(true);
        inputFieldPsychologist.text = GameManager.instance.Psychologist.Name;
        buttonAlter.gameObject.SetActive(false);
        buttonDelete.gameObject.SetActive(false);
        buttonDuplicate.gameObject.SetActive(false);
    }

    private void changePanelEditInteractable(bool flag)
    {
        inputFieldName.interactable = flag;
        inputFieldDescription.interactable = flag;
        toggleIsPublic.interactable = flag;
        toggleStatus.interactable = flag;
        buttonSelectPatient.interactable = flag;
        buttonSelectScenario.interactable = flag;
        buttonSelectWeather.interactable = flag;
        buttonAddComponent.interactable = flag;
        buttonDeleteComponent.interactable = flag;
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
        sessoes = new Session(GameManager.instance.Psychologist).SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        for(int i = 0; i< sessoes.Count; i++)
        {
            textIDB.text = sessoes[i].Id.ToString();
            textNameB.text = sessoes[i].Name;
            textPsychologistB.text = sessoes[i].Psychologist.Name;
            textPublicB.text = sessoes[i].IsPublic == 1 ? "Sim" : "Não";
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
        Session session = new Session().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        Debug.Log("SeSSAO: " + session.ListComponents.Count);

        textSessionID.text = session.Id.ToString();
        inputFieldName.text = session.Name;
        inputFieldDescription.text = session.Description;
        inputFieldPsychologist.text = session.Psychologist.Name;
        inputFieldScenario.text = session.Scenario.Name;
        inputFieldPatient.text = session.Patient.Name;
        inputFieldWeather.text = session.Weather.Name;
        toggleStatus.isOn = session.Status == 1;
        toggleIsPublic.isOn = session.IsPublic == 1;
        
        currentPatient = session.Patient;
        currentScenario = session.Scenario;
        currentWeather = session.Weather;


        LoadComponentDropdown();
        listComponent = session.ListComponents;
        foreach(KeyValuePair<int,string> par in listComponent)
        {
            Debug.Log("FOREACH> KEY: " + par.Key + "VALUE: " + par.Value);
            AddRowTableComponent(par);
        }

        //se nao for seu nao pode alterar nem deletar
        if(session.Psychologist.Id != GameManager.instance.Psychologist.Id){
            buttonAlter.gameObject.SetActive(false);
            buttonDelete.gameObject.SetActive(false);
        }
        buttonConfirm.gameObject.SetActive(false);
    }

    public void ConfirmClick()
    {
        string id = textSessionID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        bool isPubic = toggleIsPublic.isOn;
        string patName = currentPatient.Name, scenarioName = currentScenario.Name, weatherName = currentWeather.Name;
        Session session;

        if(inputFieldPsychologist.text.Trim() == "")
            Debug.Log("Erro no psicologo!");
        else if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else if(currentPatient.Id == 0)
            Debug.Log("Erro. Selecione um paciente!");
        else if(currentScenario.Id == 0)
            Debug.Log("Erro. Selecione um cenário virtual!");
        else if(currentWeather.Id == 0)
            Debug.Log("Erro. Selecione um clima!");
        else 
        {
            session = new Session(name,description,GameManager.instance.Psychologist,currentPatient,currentScenario,currentWeather, 
                status ? 1 : 0, isPubic ? 1 : 0, listComponent.Count > 0 ? listComponent : null);
            if(state == 1) //add
            {
                string returnMsg = session.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)//alter
            {
                string returnMsg = session.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
                if(returnMsg == "Ok")
                    state = 0;
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

    public void CancelClick()
    {
        if(state == 2)
        {
            textTitle.text = "Gerenciar Sessão";
            changePanelEditInteractable(false);//habilita os campos
            buttonConfirm.gameObject.SetActive(false);
            buttonAlter.gameObject.SetActive(true);
            buttonDuplicate.gameObject.SetActive(true);
            state = 0;
        }
        else
            Begin();
    }

    public void AlterClick()
    {
        state = 2;
        textTitle.text = "Gerenciar Sessão - Alterar";
        changePanelEditInteractable(true);//habilita os campos
        buttonConfirm.gameObject.SetActive(true);
        buttonAlter.gameObject.SetActive(false);
        buttonDuplicate.gameObject.SetActive(false);
    }

    public void DuplicateClick()
    {
        // setar como novo, habilitar os campos e tirar o paciente
        state = 1;
        textTitle.text = "Gerenciar Sessão - Novo";
        changePanelEditInteractable(true);
        inputFieldPsychologist.text = GameManager.instance.Psychologist.Name;
        buttonConfirm.gameObject.SetActive(true);
        buttonDuplicate.gameObject.SetActive(false);
        currentPatient = new Patient();
        inputFieldPatient.text = "";
    }

    public void AddComponentClick()
    {
        if(listComponent.Contains(arrayAllComponents[dropdownComponent.value])){
            Debug.Log("ja existe na lista! Nao add");
            return;
        }
        else {
            listComponent.Add(arrayAllComponents[dropdownComponent.value]);
            AddRowTableComponent(arrayAllComponents[dropdownComponent.value]);
        }
    }

    private void AddRowTableComponent(KeyValuePair<int,string> info)
    {
        Button newRow;
        rowComponent.gameObject.SetActive(true);
        textComponentIDB.text = info.Key.ToString();
        textComponentNameB.text = info.Value;
        newRow = Instantiate(rowComponent) as Button; 
        newRow.transform.SetParent(rowComponent.transform.parent,false);
        newRow.onClick.AddListener(() => RowClickComp(newRow));
        rowComponent.gameObject.SetActive(false);
    }

    public void RowClickComp(Button br)
    {
        //FAZER: QUANDO CLICAR DEIXAR A LINHA DE COR DIFERENTE, SE CLICAR NOVAMENTE TIRA
        int selectedComponentKey = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textComponentIDB).text);
        int i=0;
        foreach(KeyValuePair<int,string> par in listComponent)
        {
            if(par.Key == selectedComponentKey)
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
            var clone = rowsComp.transform.GetChild(selectedComponentIndex+1); //+1 porque o primeiro ta false
            Destroy(clone.gameObject);
            listComponent.RemoveAt(selectedComponentIndex);
            selectedComponentIndex = -1;
        }
    }

    public void LoadComponentDropdown()
    {
        ClearComponentTable();
        List<ScenarioComponent> lista = new ScenarioComponent().GetComponentsByScenario(currentScenario.Id);
        arrayAllComponents = new KeyValuePair<int, string>[lista.Count];
        for(int i = 0; i < lista.Count; i++)
        {
            arrayAllComponents[i] = new KeyValuePair<int, string>(lista[i].Id,lista[i].Name);
            dropdownComponent.options.Add(new Dropdown.OptionData(lista[i].Name));
        }
        dropdownComponent.value = -1;
        dropdownComponent.value = 0;
    }

    public void ClearComponentTable()
    {
        dropdownComponent.ClearOptions();
        listComponent.Clear();
        selectedComponentIndex = -1;
        //tableComplear
        var clones = new Transform[rowsComp.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsComp.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void TableLoadPatient()
    {
        List<Patient> patients;
        Button newRow;

         if (rowsClonePat != null)
        {
            var clones = new Transform[rowsPatient.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rowsPatient.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            rowPat.gameObject.SetActive(true);
        }
        
        List<Button> bts = new List<Button>();
        patients = new Patient().SearchAll(inputFieldPatSearch.text, false);
        for(int i = 0; i< patients.Count; i++)
        {
            textPatIDB.text = patients[i].Id.ToString();
            textPatNameB.text = patients[i].Name;
            textPatCpfB.text = patients[i].Cpf;
            textPatBirthdayB.text = patients[i].Birthday.ToString("dd/MM/yyyy");
            newRow = Instantiate(rowPat) as Button;
            newRow.transform.SetParent(rowPat.transform.parent,false);
            bts.Add(newRow);
        }
        rowsClonePat = rowsPatient;
        rowPat.gameObject.SetActive(false);
        AddListenerPat(bts);
    }

    public void AddListenerPat(List<Button> bts)
    {
        foreach (Button btn in bts)
        {
            btn.onClick.AddListener(() => RowClickPat(btn)); 
        }
    }

    public void RowClickPat(Button br)
    {
        selectedPatId = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textPatIDB).text);
        Patient pat = new Patient().Search(selectedPatId);
        panelPatient2.gameObject.SetActive(true);
        
        inputFieldPatName.text = pat.Name;
        inputFieldPatCpf.text = pat.Cpf;
        inputFieldPatBirthday.text = pat.Birthday.ToString("dd/MM/yyyy");
        inputFieldPatPhone.text = pat.Phone;
        inputFieldPatGender.text = pat.Gender+"";
        inputFieldPatEmail.text = pat.Email;
        inputFieldPatNote.text = pat.Note;
    }

    public void ConfirmPatClick()
    {
        currentPatient = new Patient().Search(selectedPatId);
        panelPatient2.gameObject.SetActive(false);
        panelPatient1.gameObject.SetActive(false);
        panelSelectPatient.gameObject.SetActive(false);
        inputFieldPatient.text = currentPatient.Name;
        selectedPatId = 0;
    }

    public void ClearPatTable()//chamado ao confirmar ou sair da tela1
    {
        inputFieldPatSearch.text = "";
        var clones = new Transform[rowsPatient.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsPatient.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void TableLoadScenario()
    {
        List<Scenario> scenarios;
        Button newRow;

         if (rowsCloneSce != null)
        {
            var clones = new Transform[rowsScenario.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rowsScenario.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            rowSce.gameObject.SetActive(true);
        }
        
        List<Button> bts = new List<Button>();
        scenarios = new Scenario().SearchAll(inputFieldScenarioSearch.text, false);
        for(int i = 0; i< scenarios.Count; i++)
        {
            textScenarioIDB.text = scenarios[i].Id.ToString();
            textScenarioNameB.text = scenarios[i].Name;
            textScenarioEnvTypeB.text = scenarios[i].Environment.Name;
            newRow = Instantiate(rowSce) as Button;
            newRow.transform.SetParent(rowSce.transform.parent,false);
            bts.Add(newRow);
        }
        rowsCloneSce = rowsScenario;
        rowSce.gameObject.SetActive(false);
        AddListenerSce(bts);
    }

     public void AddListenerSce(List<Button> bts)
    {
        foreach (Button btn in bts)
        {
            btn.onClick.AddListener(() => RowClickSce(btn)); 
        }
    }

    public void RowClickSce(Button br)
    {
        selectedScenarioId = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textScenarioIDB).text);
        Scenario scenario = new Scenario().Search(selectedScenarioId);
        panelScenario2.gameObject.SetActive(true);
        
        inputFieldScenarioName.text = scenario.Name;
        inputFieldScenarioEnvType.text = scenario.Environment.Name;
        inputFieldScenarioDescription.text = scenario.Description;
    }

    public void ConfirmScenarioClick()
    {
        currentScenario = new Scenario().Search(selectedScenarioId);
        panelScenario2.gameObject.SetActive(false);
        panelScenario1.gameObject.SetActive(false);
        panelSelectScenario.gameObject.SetActive(false);
        inputFieldScenario.text = currentScenario.Name;
        selectedScenarioId = 0;
    }

    public void ClearScenarioTable()//chamado ao confirmar ou sair da tela1
    {
        inputFieldScenarioSearch.text = "";
        var clones = new Transform[rowsScenario.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsScenario.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void TableLoadWeather()
    {
        List<Weather> weathers;
        Button newRow;

         if (rowsCloneWea != null)
        {
            var clones = new Transform[rowsWeather.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rowsWeather.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            rowWea.gameObject.SetActive(true);
        }
        
        List<Button> bts = new List<Button>();
        weathers = new Weather().SearchAll(inputFieldWeatherSearch.text, false);
        for(int i = 0; i< weathers.Count; i++)
        {
            textWeatherIDB.text = weathers[i].Id.ToString();
            textWeatherNameB.text = weathers[i].Name;
            textWeatherTypeB.text = weathers[i].Type.Name;
            newRow = Instantiate(rowWea) as Button;
            newRow.transform.SetParent(rowWea.transform.parent,false);
            bts.Add(newRow);
        }
        rowsCloneWea = rowsWeather;
        rowWea.gameObject.SetActive(false);
        AddListenerWe(bts);
    }

     public void AddListenerWe(List<Button> bts)
    {
        foreach (Button btn in bts)
        {
            btn.onClick.AddListener(() => RowClickWe(btn)); 
        }
    }

    public void RowClickWe(Button br)
    {
        selectedWeatherId = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textWeatherIDB).text);
        Weather weather = new Weather().Search(selectedWeatherId);
        panelWeather2.gameObject.SetActive(true);
        
        inputFieldWeatherName.text = weather.Name;
        inputFieldWeatherType.text = weather.Type.Name;
        inputFieldWeatherDescription.text = weather.Description;
    }
    public void ConfirmWeatherClick()
    {
        currentWeather = new Weather().Search(selectedWeatherId);
        panelWeather1.gameObject.SetActive(false);
        panelWeather2.gameObject.SetActive(false);
        panelSelectWeather.gameObject.SetActive(false);
        inputFieldWeather.text = currentWeather.Name;
        selectedWeatherId = 0;
    }

    public void ClearWeatherTable()//chamado ao confirmar ou sair da tela1
    {
        inputFieldWeatherSearch.text = "";
        var clones = new Transform[rowsWeather.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsWeather.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }
}
