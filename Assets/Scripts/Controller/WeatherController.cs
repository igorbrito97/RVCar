using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherController : MonoBehaviour
{
    private int state; //0 - nada, 1 - novo, 2 - alterando
    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textWeatherID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private Dropdown dropdownWeatherType;
    [SerializeField] private Slider sliderTime;
    [SerializeField] private Text textSliderTime;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Toggle toggleStatus;  
    [SerializeField] private Button buttonDelete;
    
    // tabela da tela inicial
    [SerializeField] private Text textIDB;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textWeatherTypeB;
    [SerializeField] private Text textTimeB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    //listas
    private KeyValuePair<int,string>[] arrayAllWeatherType;
     
    void Start()
    {
        inputFieldSearch.characterLimit = 60;
        inputFieldName.characterLimit = 60;
        inputFieldDescription.characterLimit = 350;
        sliderTime.wholeNumbers = true;
        sliderTime.minValue = 0;
        sliderTime.maxValue = 23;
        sliderTime.value = 12;

        Begin();
    }

    public void Begin()
    {
        state = 0;
        textTitle.text = "Gerenciar Clima";
        panelEdit.SetActive(false);
        inputFieldSearch.text = "";
        toggleStatusSearch.isOn = false;
        Clear();
    }

    public void Clear()
    {
        inputFieldName.text = "";
        inputFieldDescription.text = "";
        sliderTime.value = 12;
        toggleStatus.isOn = true;
        buttonDelete.gameObject.SetActive(false);
    }

    public void NewClick()
    {
        state = 1;
        panelEdit.gameObject.SetActive(true);
        textTitle.text = "Gerenciar Clima - Novo";
        LoadDropdown();
    }

    private void LoadDropdown()
    {
        List<WeatherType> lista = new WeatherType().SearchAll();
        dropdownWeatherType.ClearOptions();
        arrayAllWeatherType = new KeyValuePair<int, string>[lista.Count];
        for(int i = 0; i < lista.Count; i++)
        {
            arrayAllWeatherType[i] = new KeyValuePair<int, string>(lista[i].Id,lista[i].Name);
            dropdownWeatherType.options.Add(new Dropdown.OptionData(lista[i].Name));
        }
        dropdownWeatherType.value = -1;
        dropdownWeatherType.value = 0;
    }

    public void ConfirmClick()
    {
        string id = textWeatherID.text;
        string name = inputFieldName.text;
        string description = inputFieldDescription.text;
        int time = Convert.ToInt32(sliderTime.value);
        bool status = toggleStatus.isOn;
        int type_id = arrayAllWeatherType[dropdownWeatherType.value].Key;
        string type_name = arrayAllWeatherType[dropdownWeatherType.value].Value;
        Weather weather;

        if(name.Trim() == "")
            LevelManager.Instance.AlterMessage("Nome inválido. Digite um nome válido!",Color.red);
        else if(description.Trim() == "")
            LevelManager.Instance.AlterMessage("Descrição inválida. Digite uma descrição válida!",Color.red);
        else if(time < 0 || time > 23)  
            LevelManager.Instance.AlterMessage("Horário inválida. Digite um horário válido!",Color.red);
        else
        {
            weather = new Weather(name,time,new WeatherType(type_id,type_name),description,status ? 1 : 0);
            if(state == 1)
            {
                string returnMsg = weather.Insert();
                if(returnMsg.Equals("Ok"))
                {
                    LevelManager.Instance.AlterMessage("Clima inserido com sucesso!",Color.green);
                    Begin();
                    if(rowsClone != null)
                        ClearMainTable();
                }
                else
                {
                    LevelManager.Instance.AlterMessage(returnMsg, Color.red);
                }
            }
            else if(state == 2)
            {
                string returnMsg = weather.Alter(Convert.ToInt32(id));
                if(returnMsg.Equals("Ok"))
                {
                    LevelManager.Instance.AlterMessage("Clima alterado com sucesso!",Color.green);
                    Begin();
                    if(rowsClone != null)
                        ClearMainTable();
                }
                else
                {
                    LevelManager.Instance.AlterMessage(returnMsg, Color.red);
                }
            }
        }
            
    }

    public void DeleteClick()
    {
        string id = textWeatherID.text;
        bool status = toggleStatus.isOn;

        if(!status)
        {
            LevelManager.Instance.AlterMessage("Erro. Clima já desativado!", Color.red);
            return;
        }

        if(new Weather().Delete(Convert.ToInt32(id)))
        {
            Begin();
            LevelManager.Instance.AlterMessage("Clima desativado com sucesso!", Color.green);
        }
        else
        {
            LevelManager.Instance.AlterMessage("Erro ao desativar clima!", Color.red);
        }
    }

    public void TableLoad()
    {
        List<Weather> list;
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
        list = new Weather().SearchAll(inputFieldSearch.text,toggleStatusSearch.isOn);
        for(int i = 0; i < list.Count; i++)
        {
            textIDB.text = list[i].Id.ToString();
            textNameB.text = list[i].Name;
            textWeatherTypeB.text = list[i].Type.Name;
            textTimeB.text = list[i].Time.ToString();  
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
        Weather weather = new Weather().Search(Convert.ToInt32(br.gameObject.GetComponentInChildren<Text>(textIDB).text));
        buttonDelete.gameObject.SetActive(true);
        LoadDropdown();
        textTitle.text = "Gerenciar Clima - Alterar";

        textWeatherID.text = weather.Id.ToString();
        inputFieldName.text = weather.Name;
        inputFieldDescription.text = weather.Description;
        sliderTime.value = weather.Time;
        toggleStatus.isOn = weather.Status == 1;

        int i=0;
        while(i < dropdownWeatherType.options.Count && 
            arrayAllWeatherType[i].Key != weather.Type.Id)
            i++;
        dropdownWeatherType.value = i;
    }

    public void OnChangeSlider()
    {
        if(sliderTime.value > -1 && sliderTime.value < 24)
        {
            textSliderTime.text = sliderTime.value.ToString();
        }
    }

    public void ClearMainTable()
    {
        var clones = new Transform[rows.transform.childCount];
        for (var i = 1; i < clones.Length; i++)
        {
            clones[i] = rows.transform.GetChild(i);
            Destroy(clones[i].gameObject);
        }
        textIDB.text = textNameB.text = textWeatherTypeB.text = textTimeB.text = textStatusB.text = "";
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
