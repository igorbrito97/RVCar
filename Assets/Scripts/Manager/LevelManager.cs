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

		string scenarioPrefab = new VirtualObject().GetScenarioPrefabById(session.Scenario.Id);
		if(scenarioPrefab.Equals(""))
			AlterMessage("Erro ao carregar cenário!",Color.red);
		else 
		{
			SceneManager.LoadScene(scenarioPrefab);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(scene);
		GameObject drivingCar = InstantiateCar(currentSession.Car);
		//marcha
		//instanciar componentes e clima
	}

	private GameObject InstantiateCar(VirtualObject car)
	{
		Debug.Log("CARRAO: " + car.Prefab);
		//pegar posicao inicial do carro de acordo com o cenario
		switch(car.Prefab)
		{
			case "Pickup1":
				return Instantiate(carPickup1,new Vector3(0,0,0), Quaternion.identity) as GameObject;
			case "Pickup2":
				return Instantiate(carPickup2,new Vector3(0,0,0), Quaternion.identity) as GameObject;
			case "SportCoupe":
				return Instantiate(carSportCoupe,new Vector3(0,0,0), Quaternion.identity) as GameObject;
		}
		return null;
	}

	
}
