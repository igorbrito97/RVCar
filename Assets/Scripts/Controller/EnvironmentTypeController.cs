using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnvironmentTypeController : MonoBehaviour
{
    private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textTypeID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Toggle toggleStatus;  
    [SerializeField] private Button buttonDelete;
    
    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textDescriptionB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    void Start()
    {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;

        Begin();

    }

    void Update()
    {
        
    }

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Tipos de Ambientes";
        panelEdit.SetActive(false);
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
    }

    public void NewClick()
    {
        state = 1;
        panelEdit.gameObject.SetActive(true);
        textTitle.text = "Gerenciar Tipos de Ambientes - Novo";
    }

    public void ConfirmClick()
    {
        string id = textTypeID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        bool status = toggleStatus.isOn;
        EnvironmentType envType;

        if(name.Trim() == "")
            Debug.Log("Erro no nome!");
        else if(description.Trim() == "")
            Debug.Log("Erro na descrição!");
        else 
        {
            envType = new EnvironmentType(name,description,status ? 1 : 0);
            if(state == 1)
            {
                string returnMsg = envType.Insert();
                Debug.Log(returnMsg);
            }
            else if(state == 2)
            {
                string returnMsg = envType.Alter(Convert.ToInt32(id));
                Debug.Log(returnMsg);
            }
        }
    }

    public void DeleteClick()
    {
        string id = textTypeID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            Debug.Log("Ambiente ja desativado!");
            return;
        }

        if(new EnvironmentType().Delete(Convert.ToInt32(id)))
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
        List<EnvironmentType> list;
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
        list = new EnvironmentType().SearchAll(inputFieldSearch.text,toggleStatusSearch.isOn);
        for(int i = 0; i < list.Count; i++)
        {
            textIDB.text = list[i].Id.ToString();
            textNameB.text = list[i].Name;
            textDescriptionB.text = list[i].Description;
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
        EnvironmentType env = new EnvironmentType().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);
        textTitle.text = "Gerencias Tipos de Ambiente - Alterar";

        textTypeID.text = env.Id.ToString();
        inputFieldName.text = env.Name;
        inputFieldDescription.text = env.Description;
        
        toggleStatus.isOn = env.Status == 1;
    }
}
