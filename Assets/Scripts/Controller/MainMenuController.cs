using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    private GameObject active;

    [SerializeField] private Button buttonManage;
    [SerializeField] private Button buttonSession;
    [SerializeField] private Button buttonReport;
    [SerializeField] private Button buttonPatient;
    [SerializeField] private Button buttonPsychologist;
    //colocar todos os outros botoes: pensar em criar função de usuario (adm) para bloquear/mostrar conteudo apropriado 

    void Start () {
        //usuario???

        active = null;

        buttonSession.gameObject.SetActive(true);
        //buttonReport.gameObject.SetActive(true);

        buttonPatient.gameObject.SetActive(true);
        buttonPsychologist.gameObject.SetActive(true);

    }
	
	void Update () {
		
	}

    public void SwitchPanels(GameObject panel)
    {
        GameObject activeAux = active;
        if (active != null)
        {
            active.SetActive(false);

            if (activeAux != panel)
            {
                active = panel;
                active.SetActive(true);
            }
            else
            {
                active = null;
            }
        }
        else
        {
            active = panel;
            active.SetActive(true);
        }
    }

    public void LogOut()
    {
        GameManager.instance.Psychologist = null;
        GameManager.instance.Con.Close();
        //SceneManager.LoadScene(0);
    }
}
