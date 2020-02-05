using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PsychologistController : MonoBehaviour {

    // campos da tela inicial
    [SerializeField] private Text textTitle;
    [SerializeField] private InputField inputFieldSearch;
    [SerializeField] private Toggle toggleStatusSearch;

    // campos da tela de edição
    [SerializeField] private Text textPhysiotherapistID;
    [SerializeField] private InputField inputFieldName;
    [SerializeField] private InputField inputFielCpf;
    [SerializeField] private InputField inputFieldEmail;
    [SerializeField] private InputField inputFieldPhone;
    [SerializeField] private Dropdown dropdownDay;
    [SerializeField] private Dropdown dropdownMonth;
    [SerializeField] private InputField inputFieldYear;
    [SerializeField] private Dropdown dropdownGender;    
    [SerializeField] private InputField inputFieldCRP;
    [SerializeField] private Toggle toggleStatus;
    [SerializeField] private InputField inputFieldPassword;
    [SerializeField] private InputField inputFieldConfirmPassword;
    [SerializeField] private Button buttonDelete;

    // tabela da tela inicial
    [SerializeField] private Text textNameB;
    [SerializeField] private Text textCrefitoB;
    [SerializeField] private Text textUserProfileB;
    [SerializeField] private Text textStatusB;
    [SerializeField] private Text textID;
    [SerializeField] private Button row;
    [SerializeField] GameObject rows;
    private GameObject rowsClone = null;
    private List<Button> btns = new List<Button>();

    // paineis da tela
    [SerializeField] private GameObject panelEdit;

    // banco de dados
    public MySqlConnection connection;
    

    // mensagem de erro 
    [SerializeField] private Button buttonMessage;

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
