using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunSessionController : MonoBehaviour {

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

	// paineis da tela
    [SerializeField] private GameObject panelEdit;

	void Start () {
		inputFieldSearch.characterLimit = 60;

		Begin();
	}
	
	void Update () {
		
	}

	public void Begin()
	{
		textTitle.text = "Executar Sessão";
		panelEdit.gameObject.SetActive(false);
        inputFieldSearch.text = "";
		Clear();
	}

	public void Clear()
	{
		
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

	}
}
