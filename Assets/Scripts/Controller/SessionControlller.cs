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

    //tabela de fases
    private int selectedStageId = 0;
    [SerializeField] private InputField inputFieldStageSearch;
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
    //tabela componente
    [SerializeField] private Text textComponentIDB;
    [SerializeField] private Text textComponentNameB;
    [SerializeField] private Text textComponenetInfoB;
    [SerializeField] private Button rowStComp;
    [SerializeField] GameObject rowsStComp;
    


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

    //lista de objetos
    private Stage currentStage = null;
    private Patient currentPatient = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelSelectStage;
    [SerializeField] private GameObject panelStage1;
    [SerializeField] private GameObject panelStage2;
    [SerializeField] private GameObject panelSelectPatient;
    [SerializeField] private GameObject panelPatient1;
    [SerializeField] private GameObject panelPatient2;
    [SerializeField] private GameObject panelPatient3;

    void Start () {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 450;
        inputFieldPsychologist.interactable = false;
        inputFieldPatSearch.characterLimit = 60;
        inputFieldStageSearch.characterLimit = 60;

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
        buttonConfirm.gameObject.SetActive(true);
        buttonAlter.gameObject.SetActive(true);
        buttonDelete.gameObject.SetActive(true);
        buttonDuplicate.gameObject.SetActive(true);
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        currentStage = new Stage();
        selectedStageId = 0;
        currentPatient = new Patient();
        selectedPatId = 0;
        inputFieldStage.text = "";
        inputFieldPatient.text = "";
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
        sessoes = new Session(GameManager.instance.Psychologist).SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        Debug.Log("sessoes encontradas:" + sessoes.Count);
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

        textSessionID.text = session.Id.ToString();
        inputFieldName.text = session.Name;
        inputFieldDescription.text = session.Description;
        inputFieldPsychologist.text = session.Psychologist.Name;
        inputFieldStage.text = session.Stage.Name;
        inputFieldPatient.text = session.Patient.Name;
        toggleStatus.isOn = session.Status == 1;
        toggleIsPublic.isOn = session.IsPublic == 1;
        
        currentPatient = session.Patient;
        currentStage = session.Stage;

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
        string patName = currentPatient.Name, stageName = currentStage.Name;
        Session session;

        if(inputFieldPsychologist.text.Trim() == "")
            Debug.Log("Erro no psicologo!");
        else if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else if(currentPatient.Id == 0)
            Debug.Log("Erro. Selecione um paciente!");
        else if(currentStage.Id == 0)
            Debug.Log("Erro. Selecione uma fase!");
        else 
        {
            session = new Session(name,description,GameManager.instance.Psychologist,currentStage,currentPatient,status ? 1 : 0, isPubic ? 1 : 0);
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

    public void TableLoadStage()
    {
        List<Stage> stages;
        Button newRow;

         if (rowsCloneSt != null)
        {
            var clones = new Transform[rowsStage.transform.childCount];
            for (var i = 1; i < clones.Length; i++)
            {
                clones[i] = rowsStage.transform.GetChild(i);
                Destroy(clones[i].gameObject);
            }
            rowSt.gameObject.SetActive(true);
        }
        
        List<Button> bts = new List<Button>();
        stages = new Stage().SearchAll(inputFieldStageSearch.text, false); 
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
        rowsCloneSt = rowsStage;
        rowSt.gameObject.SetActive(false);
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
        selectedStageId = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textStageIDB).text);
        Stage stage = new Stage().Search(selectedStageId);
        panelStage2.gameObject.SetActive(true);
        
        inputFieldStageName.text = stage.Name;
        inputFieldStageScenario.text = stage.Scenario.Name;
        inputFieldStageWeather.text = stage.Weather.Name;
        inputFieldStageHour.text = stage.Time + "h";
        inputFieldStageDescription.text = stage.Description;
        //preencher tabela
        foreach(Configuration config in stage.Components)
        {
            Button newRow;
            rowStComp.gameObject.SetActive(true);
            textComponentIDB.text = config.Id.ToString();
            textComponentNameB.text = config.Name;
            //textComponentInfoB.text =  por enquanto sem info
            newRow = Instantiate(rowStComp) as Button; 
            newRow.transform.SetParent(rowStComp.transform.parent,false);
            //newRow.onClick.AddListener(() => RowClick2(newRow));
            rowStComp.gameObject.SetActive(false);
        }
    }
    public void ConfirmStageClick()
    {
        // add fase para currentStage
        //fechar paneis
        //colocar texto no inputdfied
        currentStage = new Stage().Search(selectedStageId);
        panelStage2.gameObject.SetActive(false);
        panelStage1.gameObject.SetActive(false);
        panelSelectStage.gameObject.SetActive(false);
        inputFieldStage.text = currentStage.Name;
        selectedStageId = 0;
    }

    public void ClearStageComponentTable()//chamado ao sair da tela2
    {
        var clones = new Transform[rowsStComp.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsStComp.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void ClearStageTable()//chamado ao confirmar ou sair da tela1
    {
        inputFieldStageSearch.text = "";
        var clones = new Transform[rowsStage.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsStage.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

}
