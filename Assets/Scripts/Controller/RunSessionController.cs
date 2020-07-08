using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunSessionController : MonoBehaviour {
    private int state = 0; // 0 nada, 2 - alter

	// campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
	[SerializeField] private Dropdown dropdownSearch;

	//tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textPatientB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

	//campos da tela de execução
    [SerializeField] private Text textSessionID;
    [SerializeField] private InputField inputFieldPatName;
    [SerializeField] private InputField inputFieldPatBirthday;
    [SerializeField] private InputField inputFieldSessionName;
    [SerializeField] private InputField inputFieldPsycName;
    [SerializeField] private InputField inputFieldSessionDescription;
    [SerializeField] private InputField inputFieldWeatherName;
    [SerializeField] private InputField inputFieldWeatherType;
    [SerializeField] private InputField inputFieldWeatherTime;
    [SerializeField] private InputField inputFieldScenarioName;
    [SerializeField] private InputField inputFieldScenarioEnvType;
    [SerializeField] private InputField inputFieldCar;
    [SerializeField] private InputField inputFieldGear;
    [SerializeField] private Button buttonAlter;
    [SerializeField] private Button buttonRun;

    //tabela Componente
    [SerializeField] private Text textIDBComponent;
    [SerializeField] private Text textNameBComponent;
    [SerializeField] private Text textQuantityBComponent;
    [SerializeField] private Button rowComponent;
    [SerializeField] private GameObject rowsComp;

    // panel informação componente
    [SerializeField] private InputField inputFieldComponentName;
    [SerializeField] private InputField inputFieldComponentQuantity;
    [SerializeField] private InputField inputFieldComponentDescription;
    [SerializeField] private RawImage imgComponent;

    //panel pre execution
    [SerializeField] private RawImage imgScenario;
    [SerializeField] private Text textScenario;

     
     //uma lista de par com 1o elemento = id e 2o = par de nome e quantidade
    private List<KeyValuePair<int,KeyValuePair<string,int>>> listComponent;

	// paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelSession;
    [SerializeField] private GameObject panelComponentInfo;
    [SerializeField] private GameObject panelPreExecution;

	void Start () {
		inputFieldSearch.characterLimit = 60;
        inputFieldPatBirthday.characterLimit = 60;
        inputFieldPatName.characterLimit = 60;
        inputFieldPsycName.characterLimit = 60;
        inputFieldScenarioEnvType.characterLimit = 60;
        inputFieldScenarioName.characterLimit = 60;
        inputFieldSessionDescription.characterLimit = 250;
        inputFieldSessionName.characterLimit = 60;
        inputFieldWeatherTime.characterLimit = 60;
        inputFieldWeatherName.characterLimit = 60;
        inputFieldWeatherType.characterLimit = 60;
        inputFieldCar.characterLimit = 60;
        inputFieldGear.characterLimit = 60;

        inputFieldPatBirthday.interactable = 
        inputFieldPatName.interactable = false;
        inputFieldPsycName.interactable = false;
        inputFieldScenarioEnvType.interactable = false;
        inputFieldScenarioName.interactable = false;
        inputFieldSessionDescription.interactable = false;
        inputFieldSessionName.interactable = false;
        inputFieldWeatherTime.interactable = false;
        inputFieldWeatherName.interactable = false;
        inputFieldWeatherType.interactable = false;
        inputFieldCar.interactable = false;
        inputFieldGear.interactable = false;
	}

	public void Begin()
	{
		textTitle.text = "Executar Sessão";
		panelEdit.gameObject.SetActive(false);
        panelComponentInfo.gameObject.SetActive(false);
        panelPreExecution.gameObject.SetActive(false);
        inputFieldSearch.text = "";
		Clear();
        buttonAlter.gameObject.SetActive(true);
        buttonRun.gameObject.SetActive(true);
	}

	public void Clear()
	{
		inputFieldPatBirthday.text = "";
        inputFieldPatName.text = "";
        inputFieldPsycName.text = "";
        inputFieldScenarioEnvType.text = "";
        inputFieldScenarioName.text = "";
        inputFieldSessionDescription.text = "";
        inputFieldSessionName.text = "";
        inputFieldWeatherTime.text = "";
        inputFieldWeatherName.text = "";
        inputFieldWeatherType.text = "";
        inputFieldCar.text = "";
        inputFieldGear.text = "";
        ClearComponentTable();
	}

	public void TableLoad()
	{
 		List<Session> sessoes;
        Button newRow;
        List<Button> bts = new List<Button>();
		int option = dropdownSearch.value; //0 = nome, 1 = paciente

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
		if(option == 0)
			sessoes = new Session(GameManager.instance.Psychologist).SearchAll(inputFieldSearch.text, false);
		else
			sessoes = new Session(GameManager.instance.Psychologist).SearchAllByPat(inputFieldSearch.text);
        for(int i = 0; i< sessoes.Count; i++)
        {
            textIDB.text = sessoes[i].Id.ToString();
            textNameB.text = sessoes[i].Name;
            textPatientB.text = sessoes[i].Patient.Name;
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
        FillPanel(session);
	}

    private void FillPanel(Session session)
    {
        textSessionID.text = session.Id.ToString();
        inputFieldPatBirthday.text = session.Patient.Birthday.Day + "/" + session.Patient.Birthday.Month + "/" + session.Patient.Birthday.Year;
        inputFieldPatName.text = session.Patient.Name;
        inputFieldPsycName.text = session.Psychologist.Name;
        inputFieldScenarioEnvType.text = session.Scenario.Environment.Name;
        inputFieldScenarioName.text = session.Scenario.Name;
        inputFieldSessionDescription.text = session.Description;
        inputFieldSessionName.text = session.Name;
        inputFieldWeatherTime.text = session.Weather.Time + "";
        inputFieldWeatherName.text = session.Weather.Name;
        inputFieldWeatherType.text = session.Weather.Type.Name;
        inputFieldCar.text = session.Car.Name;
        inputFieldGear.text = session.Gear == 0 ? "Manual" : "Automático";

        listComponent = session.ListComponents; 
        foreach(KeyValuePair<int,KeyValuePair<string,int>> item in listComponent)
        {
            AddRowTableComponent(item);
        }

        //se nao for seu nao pode alterar nem deletar
        if(session.Psychologist.Id != GameManager.instance.Psychologist.Id){
            buttonAlter.gameObject.SetActive(false);
            buttonRun.gameObject.SetActive(false);
        }
    }

    public void AlterClick()
    {
        SessionControlller controller = panelSession.GetComponent<SessionControlller>();
        panelEdit.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        controller.AlterClickRunSessionController(new Session().Search(Convert.ToInt32(textSessionID.text)));
    }

    public void RunClick()
    {
        int id = Convert.ToInt32(textSessionID.text);
        textScenario.text = "Cenário: " + inputFieldScenarioName.text;
        panelPreExecution.gameObject.SetActive(true);
        imgScenario.texture = Resources.Load(new VirtualObject().GetScenarioImageByScenarioId(id)) as Texture;
    }

    public void CancelPreExecutionClick()
    {
        panelPreExecution.gameObject.SetActive(false);
        imgScenario.texture = null;
    }

    public void ExecuteClick()
    {
        LevelManager.Instance.LoadSession(new Session().Search(Convert.ToInt32(textSessionID.text)));
    }

    private void AddRowTableComponent(KeyValuePair<int,KeyValuePair<string,int>> item)
    {
        Button newRow;
        rowComponent.gameObject.SetActive(true);
        textIDBComponent.text = item.Key.ToString();
        textNameBComponent.text = item.Value.Key;
        textQuantityBComponent.text = item.Value.Value.ToString();
        Debug.Log("2QUANT: " + textQuantityBComponent.text);
        newRow = Instantiate(rowComponent) as Button; 
        newRow.transform.SetParent(rowComponent.transform.parent,false);
        newRow.onClick.AddListener(() => RowClickComp(newRow,item.Value.Value));
        rowComponent.gameObject.SetActive(false);
    }

    public void ClearComponentTable()
    {
        listComponent.Clear();
        //tableComp clear
        var clones = new Transform[rowsComp.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsComp.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void RowClickComp(Button br, int quant)
    {
        //mostra imagem com componente
        panelComponentInfo.gameObject.SetActive(true);
        Debug.Log("QUANT: " + br.gameObject.GetComponentInChildren<Text>(textQuantityBComponent).text);
        ScenarioComponent component = new ScenarioComponent().SimpleSearch(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDBComponent).text));
        inputFieldComponentName.text = component.Name;
        inputFieldComponentQuantity.text = quant.ToString();
        inputFieldComponentDescription.text = component.Description;

        imgComponent.texture = Resources.Load(new VirtualObject().GetComponentImageByScenarioId(component.Id)) as Texture;
    }

    public void OkComponentInfoClick()
    {
        panelComponentInfo.gameObject.SetActive(false);
        inputFieldComponentName.text = "";
        inputFieldComponentQuantity.text = "";
        inputFieldComponentDescription.text = "";

        imgComponent.texture = null;
    
    }

    public void ExecuteClickSessionController(Session session)
    {
        this.gameObject.SetActive(true);
        panelEdit.gameObject.SetActive(true);
        FillPanel(session);
    }
    public void ClearMainTable()
    {
        var clones = new Transform[rows.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rows.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
        textIDB.text = textNameB.text = textPatientB.text = "";
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
