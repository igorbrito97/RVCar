using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmSessionAction : MonoBehaviour
{
    private int action; // 0 - restar / 1 - quit
	[SerializeField] private GameObject pausePanel;

    public int Action { get => action; set => action = value; }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //se é restart ou exit e chama
            //		LevelManager.Instance.ExitSession();
            if(action == 0)
                LevelManager.Instance.RestartSession();
            else if(action == 1)
                LevelManager.Instance.ExitSession();

            this.gameObject.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
            pausePanel.gameObject.SetActive(false);
        }
    }
}
