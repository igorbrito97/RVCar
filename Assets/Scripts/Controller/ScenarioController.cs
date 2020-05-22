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
    
    //tabela para componente
    [SerializeField] private Dropdown dropdownComponent;
    [SerializeField] private Button deleteComponentButton;
    [SerializeField] private Text textIDBComponent;
    [SerializeField] private Text textNameBComponent;
    [SerializeField] private Button rowComponent;
    [SerializeField] private GameObject rowsComp;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelTableComponent;
    [SerializeField] private GameObject panelVirtualObj;

    //listas 
    private KeyValuePair<int,string>[] arrayAllEnvType;
    private KeyValuePair<int,string>[] arrayAllComponent;
    private List<KeyValuePair<int,string>> listComponent = new List<KeyValuePair<int, string>>();
    private int selectedComponentIndex;

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
        panelTableComponent.SetActive(false);
        panelVirtualObj.SetActive(false);
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
        listComponent.Clear();
        selectedComponentIndex = -1;
        //tableClear
        var clones = new Transform[rowsComp.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rowsComp.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
    }

    public void NewClick()
    {
        state = 1;
        panelEdit.gameObject.SetActive(true);
        panelTableComponent.SetActive(true);
        panelVirtualObj.SetActive(true);
        textTitle.text = "Gerenciar Cenário - Novo";
        LoadDropdown();
    }

    private void LoadDropdown()
    {
        List<EnvironmentType> listaET = new EnvironmentType().SearchAll("",false);
        dropdownEnvironmentType.ClearOptions();
        arrayAllEnvType = new KeyValuePair<int, string>[listaET.Count];
        for(int i = 0; i < listaET.Count; i++)
        {
            arrayAllEnvType[i] = new KeyValuePair<int, string>(listaET[i].Id,listaET[i].Name);
            dropdownEnvironmentType.options.Add(new Dropdown.OptionData(listaET[i].Name));
        }
        dropdownEnvironmentType.value = -1;
        dropdownEnvironmentType.value = 0;

        List<ScenarioComponent> listaC = new ScenarioComponent().SearchAll("",false);
        dropdownComponent.ClearOptions();
        arrayAllComponent = new KeyValuePair<int, string>[listaC.Count];
        for(int i = 0; i < listaC.Count; i++)
        {
            arrayAllComponent[i] = new KeyValuePair<int, string>(listaC[i].Id,listaC[i].Name);
            dropdownComponent.options.Add(new Dropdown.OptionData(listaC[i].Name));
        }
        dropdownComponent.value = -1;
        dropdownComponent.value = 0;
    }

    public void ConfirmClick()
    {
        string id = textScenarioID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        int envType_id = arrayAllEnvType[dropdownEnvironmentType.value].Key;
        Scenario scenario;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else
        {
            scenario = new Scenario(name,new EnvironmentType(envType_id),description,status ? 1 : 0,listComponent.Count > 0 ? listComponent : null);
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
        panelTableComponent.SetActive(true);
        panelVirtualObj.SetActive(true);
        LoadDropdown();
        textTitle.text = "Gerenciar Cenário - Alterar";

        textScenarioID.text = scenario.Id.ToString();
        inputFieldName.text = scenario.Name;
        inputFieldDescription.text = scenario.Description;
        toggleStatus.isOn = scenario.Status == 1;

        int i=0;
        while(i < dropdownEnvironmentType.options.Count && 
            arrayAllEnvType[i].Key != scenario.Environment.Id)
            i++;
        dropdownEnvironmentType.value = i;

        listComponent = scenario.ListComponents;
        foreach(KeyValuePair<int,string> par in listComponent)
        {
            AddRowTableComponent(par);
        }
    }

    public void AddComponentClick()
    {
        if(listComponent.Contains(arrayAllComponent[dropdownComponent.value])){
            Debug.Log("ja existe na lista! Nao add");
            return;
        }
        else {
            listComponent.Add(arrayAllComponent[dropdownComponent.value]);
            AddRowTableComponent(arrayAllComponent[dropdownComponent.value]);
        }
    }

    private void AddRowTableComponent(KeyValuePair<int,string> info)
    {
        Button newRow;
        rowComponent.gameObject.SetActive(true);
        textIDBComponent.text = info.Key.ToString();
        textNameBComponent.text = info.Value;
        newRow = Instantiate(rowComponent) as Button; 
        newRow.transform.SetParent(rowComponent.transform.parent,false);
        newRow.onClick.AddListener(() => RowClick2(newRow));
        rowComponent.gameObject.SetActive(false);
    }

    public void RowClick2(Button br)
    {
        //FAZER: QUANDO CLICAR DEIXAR A LINHA DE COR DIFERENTE, SE CLICAR NOVAMENTE TIRA
        int selectedComponentKey = Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDBComponent).text);
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
        Debug.Log("DELETECLICK: " + selectedComponentIndex);
        if(selectedComponentIndex < 0)
            Debug.Log("Erro ao excluir!");
        else {
            var clone = rowsComp.transform.GetChild(selectedComponentIndex+1); //+1 porque o primeiro ta false
            Destroy(clone.gameObject);
            listComponent.RemoveAt(selectedComponentIndex);
            selectedComponentIndex = -1;
        }
    }
}
