using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
	public MainCarController carController;
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject PausePanel;
    void Start()
    {
        StartPanel.SetActive(true);
        PausePanel.SetActive(false);
        carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(StartPanel.activeSelf)
                StartPanel.SetActive(false);
            else if(!PausePanel.activeSelf)
            {
                PausePanel.SetActive(true);
                carController.isPaused = true;
            }
        }
        else if(LogitechGSDK.LogiButtonIsPressed(0, 10)) //start button
        {
            if(StartPanel.activeSelf)
                StartPanel.SetActive(false);
        }
    }
}
