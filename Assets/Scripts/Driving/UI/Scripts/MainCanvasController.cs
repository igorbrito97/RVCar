using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
	public MainCarController carController;
    [SerializeField] GameObject PausePanel;
    void Start()
    {
        PausePanel.SetActive(false);
        carController = GameObject.FindObjectOfType (typeof(MainCarController))as MainCarController;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!PausePanel.activeSelf)
            {
                PausePanel.SetActive(true);
                carController.isPaused = true;
            }
        }
    }
}
