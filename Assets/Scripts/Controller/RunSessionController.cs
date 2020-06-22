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
    [SerializeField] private InputField inputFieldWeatherInfo;
    [SerializeField] private InputField inputFieldScenarioName;
    [SerializeField] private InputField inputFieldScenarioEnvType;
    [SerializeField] private InputField inputFieldCar;
    [SerializeField] private InputField inputFieldGear;
    [SerializeField] private Button buttonAlter;
    [SerializeField] private Button buttonRun;
    [SerializeField] private Button buttonDelete;

    //tabela Componente
    [SerializeField] private Text textIDBComponent;
    [SerializeField] private Text textNameBComponent;
    [SerializeField] private Button rowComponent;
    [SerializeField] private GameObject rowsComp;
    private List<KeyValuePair<int,string>> listComponent = new List<KeyValuePair<int, string>>();

	// paineis da tela
    [SerializeField] private GameObject panelEdit;

	void Start () {
		inputFieldSearch.characterLimit = 60;
        inputFieldPatBirthday.characterLimit = 60;
        inputFieldPatName.characterLimit = 60;
        inputFieldPsycName.characterLimit = 60;
        inputFieldScenarioEnvType.characterLimit = 60;
        inputFieldScenarioName.characterLimit = 60;
        inputFieldSessionDescription.characterLimit = 250;
        inputFieldSessionName.characterLimit = 60;
        inputFieldWeatherInfo.characterLimit = 60;
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
        inputFieldWeatherInfo.interactable = false;
        inputFieldWeatherName.interactable = false;
        inputFieldWeatherType.interactable = false;
        inputFieldCar.interactable = false;
        inputFieldGear.interactable = false;
	}

	public void Begin()
	{
		textTitle.text = "Executar Sessão";
		panelEdit.gameObject.SetActive(false);
        inputFieldSearch.text = "";
		Clear();
        buttonAlter.gameObject.SetActive(true);
        buttonDelete.gameObject.SetActive(true);
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
        inputFieldWeatherInfo.text = "";
        inputFieldWeatherName.text = "";
        inputFieldWeatherType.text = "";
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
        inputFieldWeatherInfo.text = session.Weather.Info + "";
        inputFieldWeatherName.text = session.Weather.Name;
        inputFieldWeatherType.text = session.Weather.Type.Name;

        listComponent = session.ListComponents; 
        foreach(KeyValuePair<int,string> par in listComponent)
        {
            AddRowTableComponent(par);
        }

        //ver botoes
    }

    public void AlterClick()
    {

    }

    public void RunClick()
    {
    }

    public void DeleteClick()
    {

    }

    private void AddRowTableComponent(KeyValuePair<int,string> info)
    {
        Button newRow;
        rowComponent.gameObject.SetActive(true);
        textIDBComponent.text = info.Key.ToString();
        textNameBComponent.text = info.Value;
        newRow = Instantiate(rowComponent) as Button; 
        newRow.transform.SetParent(rowComponent.transform.parent,false);
        newRow.onClick.AddListener(() => RowClickComp(newRow));
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

    public void RowClickComp(Button br)
    {

    }

        public void ExecuteClickSessionController(Session session)
    {
        this.gameObject.SetActive(true);
        panelEdit.gameObject.SetActive(true);
        FillPanel(session);
    }

    void OnDisable()
    {
        Begin();
    }

}
