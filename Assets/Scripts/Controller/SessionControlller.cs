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
    [SerializeField] private InputField inputFieldStage;
    [SerializeField] private Button buttonSelectPatient;
    [SerializeField] private Button buttonSelectStage;

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

    //tabela de pacientes
    [SerializeField] private Text textPatIDB;
    [SerializeField] private Text textPatNameB;
    [SerializeField] private Text textPatCpfB;
    [SerializeField] private Text textPatBirthdayB;
    [SerializeField] private Button rowPat;
    [SerializeField] GameObject rowsPatient;
    private GameObject rowsClonePat = null;

    //tabela de fases
    [SerializeField] private Text textStageIDB;
    [SerializeField] private Text textStageNameB;
    [SerializeField] private Text textStageScenarioB;
    [SerializeField] private Text textStageWeatherB;
    [SerializeField] private Text textStageTimeB;
    [SerializeField] private Button rowSt;
    [SerializeField] GameObject rowsStage;
    private GameObject rowsCloneSt = null;

    //campos tela de fases
    [SerializeField] private InputField inputFieldStageName;
    [SerializeField] private InputField inputFieldStageScenario;
    [SerializeField] private InputField inputFieldStageWeather;
    [SerializeField] private InputField inputFieldStageHour;
    [SerializeField] private InputField inputFieldStageDescription;
    //tabela
    [SerializeField] private Text textComponentNameB;
    [SerializeField] private Text textComponenetInfoB;
    [SerializeField] private Button rowStComp;
    [SerializeField] GameObject rowsStComp;

    //lista de objetos
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelSelectStage;
    [SerializeField] private GameObject panelStage1;
    [SerializeField] private GameObject panelStage2;
    [SerializeField] private GameObject panelSelectPatient;
    [SerializeField] private GameObject panelPatient1;
    [SerializeField] private GameObject panelPatient2;
    [SerializeField] private GameObject panelPatient3;

    // banco de dados
    public MySqlConnection connection;

    void Start () {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 450;
        inputFieldPsychologist.interactable = false;

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
        panelSelectStage.gameObject.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
        changePanelEditInteractable(false);
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
    }

    public void New()
    {
        state = 1;
        textTitle.text = "Gerenciar Sessão - Novo";
        panelEdit.gameObject.SetActive(true);
        Clear();
        //preenche psicologo
        changePanelEditInteractable(true);
    }

    private void changePanelEditInteractable(bool flag)
    {
        inputFieldName.interactable = flag;
        inputFieldDescription.interactable = flag;
        toggleIsPublic.interactable = flag;
        toggleStatus.interactable = flag;
        buttonSelectPatient.interactable = flag;
        buttonSelectStage.interactable = flag;
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
            // textIDB.text = sessoes[i].Id.ToString();
            // textNameB.text = sessoes[i].Name;
            // textScenarioB.text = sessoes[i].Scenario.Name;
            // textWeatherB.text = sessoes[i].Weather.Name;
            // textTimeB.text = sessoes[i].Time.ToString() + " h";
            // textStatusB.text = sessoes[i].Status == 1 ? "Ativo" : "Desativado";
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

        textSessionID.text = session.Id.ToString();
        inputFieldName.text = session.Name;
        inputFieldDescription.text = session.Description;
        toggleStatus.isOn = session.Status == 1;
        
    }

    public void ConfirmClick()
    {
        string id = textSessionID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        Session session;


        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else 
        {
            session = new Session();
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

    public void SelectPatientClick()
    {
        panelSelectPatient.gameObject.SetActive(true);
        panelPatient1.gameObject.SetActive(true);
    }

    public void SelectStageClick()
    {
        panelSelectStage.gameObject.SetActive(true);
        panelStage1.gameObject.SetActive(true);
        panelStage2.gameObject.SetActive(false);
    }

    public void TableLoadPatient()
    {

    }

    public void TableLoadStage()
    {
        Debug.Log("buscando stages");
        List<Stage> stages;
        Button newRow;

         if (rowsCloneSt != null)
        {
            var clones = new Transform[rows.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rows.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            rowsStage.gameObject.SetActive(true);
        }
        
        List<Button> bts = new List<Button>();
        stages = new Stage().SearchAll(inputFieldSearch.text, false); 
        Debug.Log("stages"+     stages.Count);
        for(int i = 0; i< stages.Count; i++)
        {
            textStageIDB.text = stages[i].Id.ToString();
            textStageNameB.text = stages[i].Name;
            textStageScenarioB.text = stages[i].Scenario.Name;
            textStageWeatherB.text = stages[i].Weather.Name;
            textStageTimeB.text = stages[i].Time.ToString() + " h";
            newRow = Instantiate(rowSt) as Button;
            newRow.transform.SetParent(rowSt.transform.parent,false);
            bts.Add(newRow);
        }
        rowsCloneSt = rows;
        rowsStage.gameObject.SetActive(false);
        AddListenerSt(bts);
    }

     public void AddListenerSt(List<Button> bts)
    {
        foreach (Button btn in bts)
        {
            btn.onClick.AddListener(() => RowClickSt(btn)); 
        }
    }

    public void RowClickSt(Button br)
    {
        Stage stage = new Stage().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textStageIDB).text));
        panelStage2.gameObject.SetActive(true);
        
        inputFieldStageName.text = "";
        inputFieldStageScenario.text = "";
        inputFieldStageWeather.text = "";
        inputFieldStageHour.text = "";
        inputFieldStageDescription.text = "";
        //preencher tabela

    }

}
