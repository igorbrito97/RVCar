using UnityEngine;

public class MainOverlayController : MonoBehaviour
{
    public MainCarController carController;
    [SerializeField] GameObject StartPanel;
    void Start()
    {
        StartPanel.SetActive(true);
        carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
    }

    void Update()
    {
        if(LogitechGSDK.LogiButtonIsPressed(0, 10)) //start button
        {
            if(StartPanel.activeSelf)
                StartPanel.SetActive(false);
        }
    }
}
