using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentController : MonoBehaviour
{
     private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textComponentID;
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
    
    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textDescriptionB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    
    //objeto virtual
    [SerializeField] private Dropdown dropdownSelectObject;
    [SerializeField] private RawImage objImage;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;
    [SerializeField] private GameObject panelTableComponent;
    [SerializeField] private GameObject panelVirtualObj;

    private KeyValuePair<int,string>[] arrayAllScenarios;
    private KeyValuePair<int,string>[] arrayAllVirtualObj;
    private List<KeyValuePair<int,string>> listScenario = new List<KeyValuePair<int, string>>();
    private int selectedScenarioIndex;

    void Start()
    {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;

        Begin();
    }

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Componente";
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
        listScenario.Clear();
        selectedScenarioIndex = -1;
        objImage.texture = null;
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
        textTitle.text = "Gerenciar Componente - Novo";
        panelTableComponent.SetActive(true);
        panelVirtualObj.SetActive(true);
        LoadDropdown();
    }

    public void LoadDropdown()
    {
        Scenario scenario = new Scenario();
        List<Scenario> lista = scenario.SearchAll("",false);
        dropdownScenario.ClearOptions();
        arrayAllScenarios = new KeyValuePair<int, string>[lista.Count];
        for(int i = 0; i < lista.Count; i++)
        {
            arrayAllScenarios[i] = new KeyValuePair<int, string>(lista[i].Id,lista[i].Name);
            dropdownScenario.options.Add(new Dropdown.OptionData(lista[i].Name));
        }
        dropdownScenario.value = -1;
        dropdownScenario.value = 0;

        List<VirtualObject> listaVO = new VirtualObject().SearchAllComponent();
        dropdownSelectObject.ClearOptions();
        arrayAllVirtualObj = new KeyValuePair<int, string>[listaVO.Count];
        for(int i = 0; i < listaVO.Count; i++)
        {
            arrayAllVirtualObj[i] = new KeyValuePair<int, string>(listaVO[i].Id,listaVO[i].Name);
            dropdownSelectObject.options.Add(new Dropdown.OptionData(listaVO[i].Name));
        }
        dropdownSelectObject.value = -1;
    }

    public void ConfirmClick()
    {
        string id = textComponentID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        int objComp_id = arrayAllVirtualObj[dropdownSelectObject.value].Key;
        bool status = toggleStatus.isOn;
        ScenarioComponent comp;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        // else if(listScenario.Count == 0)
        //     Debug.Log("Erro nos cenários. É necessário pelo menos 1!");
        else 
        {
            comp = new ScenarioComponent(name, description, status ? 1 : 0, listScenario.Count > 0 ? listScenario : null,
                new VirtualObject(objComp_id));
            if(state == 1) //add
            {
                string returnMsg = comp.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)//alter
            {
                string returnMsg = comp.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
            }
        }
    }

    public void DeleteClick()
    {
        string id = textComponentID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            Debug.Log("Configuração ja desativada!");
            return;
        }

        if(new ScenarioComponent().Delete(Convert.ToInt32(id)))
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
        List<ScenarioComponent> comps;
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
        comps = new ScenarioComponent().SearchAll(inputFieldSearch.text, toggleStatusSearch.isOn);
        for(int i = 0; i < comps.Count; i++)
        {
            textIDB.text = comps[i].Id.ToString();
            textNameB.text = comps[i].Name;
            textDescriptionB.text = comps[i].Description;
            textStatusB.text = comps[i].Status == 1 ? "Ativo" : "Desativado";
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
        ScenarioComponent comp = new ScenarioComponent().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);

        textComponentID.text = comp.Id.ToString();
        inputFieldName.text = comp.Name;
        inputFieldDescription.text = comp.Description;
        
        toggleStatus.isOn = comp.Status == 1;

        textTitle.text = "Gerenciar Componente - Alterar";
        panelTableComponent.SetActive(true);
        panelVirtualObj.SetActive(true);
        LoadDropdown();

        int i=0;
        while(i < dropdownSelectObject.options.Count && 
            arrayAllVirtualObj[i].Key != comp.ObjComponent.Id)
            i++;
        dropdownSelectObject.value = i;

        listScenario = comp.ListScenarios;
        foreach(KeyValuePair<int,string> par in listScenario)
        {
            AddRowTableScenario(par);
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

    public void ChangeImage()
    {
        if(dropdownSelectObject.value > -1)
        {
            VirtualObject obj = new VirtualObject().SearchComponent(arrayAllVirtualObj[dropdownSelectObject.value].Key);
            objImage.texture = Resources.Load(obj.File) as Texture;
        }
    }
}
