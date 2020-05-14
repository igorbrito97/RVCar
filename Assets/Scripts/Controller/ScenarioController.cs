using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioController : MonoBehaviour
{
    private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textScenarioID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFieldFile;
    [SerializeField] private Button buttonSelectFile;
    [SerializeField] private Dropdown dropdownEnvironmentType;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Toggle toggleStatus;  
    [SerializeField] private Button buttonDelete;
    
    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textEnvironmentTypeB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    //listas 
    private KeyValuePair<int,string>[] arrayAllEnvType;

    void Start()
    {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;

        Begin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Cenário";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        inputFieldFile.text = "";
        toggleStatus.isOn = true;
        buttonDelete.gameObject.SetActive(false);
    }

    public void NewClick()
    {
        state = 1;
        panelEdit.gameObject.SetActive(true);
        textTitle.text = "Gerenciar Cenário - Novo";
        LoadDropdown();
    }

    private void LoadDropdown()
    {
        List<EnvironmentType> lista = new EnvironmentType().SearchAll("",false);
        dropdownEnvironmentType.ClearOptions();
        arrayAllEnvType = new KeyValuePair<int, string>[lista.Count];
        for(int i = 0; i < lista.Count; i++)
        {
            arrayAllEnvType[i] = new KeyValuePair<int, string>(lista[i].Id,lista[i].Name);
            dropdownEnvironmentType.options.Add(new Dropdown.OptionData(lista[i].Name));
        }
        dropdownEnvironmentType.value = -1;
        dropdownEnvironmentType.value = 0;
    }

    public void ConfirmClick()
    {
        string id = textScenarioID.text;
        string name = inputFieldName.text;
        string file = inputFieldFile.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        int envType_id = arrayAllEnvType[dropdownEnvironmentType.value].Key;
        Scenario scenario;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else if(file.Trim() == "")  
            Debug.Log("Erro no arquivo!");
        else
        {
            scenario = new Scenario(name,file,new EnvironmentType(envType_id),description,status ? 1 : 0);
            if(state == 1)
            {
                string returnMsg = scenario.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)
            {
                string returnMsg = scenario.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
            }
        }
    }

    public void DeleteClick()
    {
        string id = textScenarioID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            Debug.Log("Cenario ja desativado!");
            return;
        }

        if(new Scenario().Delete(Convert.ToInt32(id)))
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
        List<Scenario> list;
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
        list = new Scenario().SearchAll(inputFieldSearch.text,toggleStatusSearch.isOn);
        for(int i = 0; i < list.Count; i++)
        {
            textIDB.text = list[i].Id.ToString();
            textNameB.text = list[i].Name;
            textEnvironmentTypeB.text = list[i].Environment.Name;
            textStatusB.text = list[i].Status == 1 ? "Ativo" : "Desativado";
            newRow = Instantiate(row) as Button;
            newRow.transform.SetParent(row.transform.parent,false);
            btns.Add(newRow);
        }

        rowsClone = rows;
        row.gameObject.SetActive(false);
        AddListener(btns);
    }
    
    public void AddListener(List<Button> btns)
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => RowClick(btn));
        }
    }

    public void RowClick(Button br)
    {
        state = 2;
        Scenario scenario = new Scenario().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);
        LoadDropdown();
        textTitle.text = "Gerenciar Cenário - Alterar";

        textScenarioID.text = scenario.Id.ToString();
        inputFieldName.text = scenario.Name;
        inputFieldDescription.text = scenario.Description;
        inputFieldFile.text = scenario.File;
        toggleStatus.isOn = scenario.Status == 1;

        int i=0;
        while(i < dropdownEnvironmentType.options.Count && 
            arrayAllEnvType[i].Key != scenario.Environment.Id)
            i++;
        dropdownEnvironmentType.value = i;
    }
}
