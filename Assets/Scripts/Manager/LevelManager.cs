using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance = null;
    [SerializeField] private Button buttonMessage;
    [SerializeField] private Text textMessage;
	private bool msgStatus;
    private float startTime;

	//SCENE OBJECTS
	//CAR
	[SerializeField] GameObject carPickup1;
	[SerializeField] GameObject carPickup2;
	[SerializeField] GameObject carSportCoupe;

	//COMPONENTS
	[SerializeField] GameObject componentCar;
	[SerializeField] GameObject componentGarage;

	//para execução
	public static Session currentSession;

    // Use this for initialization
    void Start () {
        if (Instance == null)
        {
            msgStatus = false;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(msgStatus)
        {
            if(Time.time - startTime > 5)
            {
                msgStatus = false;
                buttonMessage.GetComponent<RectTransform>().localPosition =
                    new Vector3(buttonMessage.GetComponent<RectTransform>().localPosition.x,
                    -775f, 0f);
            }
        }
	}

    public void AlterMessage(string text, Color color)
    {
        var colors = buttonMessage.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        buttonMessage.colors = colors;

        textMessage.text = text;
		textMessage.color = Color.black;
        buttonMessage.GetComponent<RectTransform>().localPosition =
            new Vector3(buttonMessage.GetComponent<RectTransform>().localPosition.x,
            -340f, 0f);

        msgStatus = true;
        startTime = Time.time;
    }

	public void LoadSession(Session session)
	{
		// selecionar cenario, carro com marcha correta, componentes, clima
		currentSession = session;
		Debug.Log("EXEcutando: " + session.Id + " - " + session.Name);

		string scenarioPrefab = new VirtualObject().GetScenarioPrefabById(session.Scenario.Id);
		if(scenarioPrefab.Equals(""))
			AlterMessage("Erro ao carregar cenário!",Color.red);
		else 
		{
			SceneManager.LoadScene(scenarioPrefab);
			GameObject drivingCar = GetCar(session.Car);
			Instantiate(drivingCar,new Vector3(0,0,0), Quaternion.identity);
		}
	}

	private GameObject GetCar(VirtualObject car)
	{
		switch(car.Prefab)
		{
			case "Pickup1":
				return carPickup1;
			case "Pickup2":
				return carPickup2;
			case "SportCoupe":
				return carSportCoupe;
		}
		return null;
	}
}
