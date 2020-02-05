using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageControlller : MonoBehaviour {

    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private Toggle toggleStatusSearch;
    [SerializeField] private InputField inputFieldSearch;

    // campos da tela de edição
    [SerializeField] private Text textStageID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private InputField inputFieldDescription;
    [SerializeField] private Dropdown dropdownScenario;
    [SerializeField] private Dropdown dropdownWeather;
    [SerializeField] private Dropdown dropdownElements;
    [SerializeField] private Dropdown dropdownTime;
    [SerializeField] private Button buttonAdd;

    // tabela da tela inicial
    [SerializeField] private Text textID;
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textTypeB;
    [SerializeField] private Text textDescriptionB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Button buttonDelete;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;

    // Tabela de movimentos da fase
    private GameObject rowsClone2 = null;
    private List<Button> btns2 = new List<Button>();
    [SerializeField] private Button row2;
    [SerializeField] GameObject rows2;
    [SerializeField] private Text textPosB2;
    [SerializeField] private Text textMovementB2;
    [SerializeField] private Text textCategoryB2;
    [SerializeField] private Text textDurationB2;

    //
    [SerializeField] private GameObject panelAlterAdd;

    // AddMovementPanel
    [SerializeField] private GameObject panelStageMovementAdd;
    [SerializeField] private InputField inputFieldSearchAdd; // sm = stage movement
    [SerializeField] private InputField inputFieldNameSMPAdd;
    [SerializeField] private InputField inputFieldCategorySMPAdd;
    [SerializeField] private InputField inputFieldDurationSMPAdd;
    [SerializeField] private InputField inputFieldRestingSMPAdd;
    [SerializeField] private Slider sliderIntensitySMPAdd;
    [SerializeField] private Text textTitleAlterAdd;
    [SerializeField] private Text textIDSMADD;
    [SerializeField] private Text textMovementSMADD;
    [SerializeField] private Text textCategorySMADD;
    [SerializeField] private Button rowAdd;
    [SerializeField] GameObject rowsAdd;
    private GameObject rowsCloneAdd = null;
    private List<Button> btnsAdd = new List<Button>();

    // AlterMovementsPanel
    [SerializeField] private GameObject panelStageMovementAlter;
    [SerializeField] private InputField inputFieldNameSMPAlter;
    [SerializeField] private InputField inputFieldCategorySMPAlter;
    [SerializeField] private InputField inputFieldDurationSMPAlter;
    [SerializeField] private InputField inputFieldRestingSMPAlter;
    [SerializeField] private Slider sliderIntensitySMPAlter;


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FirstClick()
    {

    }

    public void ShowPanel(bool flag)
    {
        if(panelEdit!=null)
            panelEdit.gameObject.SetActive(flag);

    }
}
