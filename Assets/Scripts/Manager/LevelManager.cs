using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance = null;
    [SerializeField] private Button buttonMessage;
    [SerializeField] private Text textMessage;
	private bool msgStatus;
    private float startTime;
	private GameObject mainCar;

	//SCENE OBJECTS
	//CAR
	[SerializeField] GameObject carPickup1;
	[SerializeField] GameObject carPickup2;
	[SerializeField] GameObject carSportCoupe;

	//COMPONENTS
	[SerializeField] GameObject componentGarage;
	[SerializeField] GameObject[] componentMovingCars; // carros que ficam movimentando
	[SerializeField] GameObject[] componentParkedCars; // carros que ficam movimentando

	//para execução
	public static Session currentSession;

    // Use this for initialization
    void Start () {
		mainCar = null;
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
		//instanciar componentes e clima
		
		//carro
		mainCar = InstantiateCar(currentSession.Car);
		//marcha

		for(int i=0;i<currentSession.ListComponents.Count;i++)
		{
			if(currentSession.ListComponents[i].Key == 6)//garage
			{
				KeyValuePair<Vector3,Quaternion> position = 
					GetPosition(new VirtualObject().GetGaragePositionByScenarioId(currentSession.Scenario.Id));

                GameObject garage = Instantiate(componentGarage, position.Key, position.Value) as GameObject;
			}
			else if(currentSession.ListComponents[i].Key == 5)//moving car
			{
				//ver quantidade, instaciar(pos inicial), selecionar o caminho e qual carro para todos
				// as posicoes estao no banco separadas por /
				//CAT DEALERS: 1:37
				List<KeyValuePair<Vector3,Quaternion>> listCarPos = GetCarInitialPositions(currentSession.ListComponents[i].Key);
				for(int j = 0; j < currentSession.ListComponents[i].Value.Value; j++)
				{
					Instantiate(componentMovingCars[0], listCarPos[j].Key, listCarPos[j].Value);
				}
			}
			else if(currentSession.ListComponents[i].Key == 7)//parked car
			{
				List<KeyValuePair<Vector3,Quaternion>> listCarPos = GetCarInitialPositions(currentSession.ListComponents[i].Key);
				listCarPos = Shuffle(listCarPos);
				for(int j = 0; j < currentSession.ListComponents[i].Value.Value; j++)
				{
					Instantiate(componentParkedCars[Random.Range(0,componentParkedCars.Length)], listCarPos[j].Key, listCarPos[j].Value);
				}
			}
		}

	}

	private List<KeyValuePair<Vector3,Quaternion>> Shuffle(List<KeyValuePair<Vector3,Quaternion>> list)
	{  
		int n = list.Count;  
		while (n > 1) 
		{  
			KeyValuePair<Vector3,Quaternion> item;
			n--;  
			int k = Random.Range(0,n + 1);  
			item = list[k];  
			list[k] = list[n];  
			list[n] = item;  
		}  
		return list;
	}

	private GameObject InstantiateCar(VirtualObject car)
	{
		//pegar posicao inicial do carro de acordo com o cenario
		KeyValuePair<Vector3,Quaternion> initialPosition = GetPosition(new VirtualObject().GetCarPositionByScenarioId(currentSession.Scenario.Id));
		switch(car.Prefab)
		{
			case "Pickup1":
				return Instantiate(carPickup1, initialPosition.Key, initialPosition.Value) as GameObject;
			case "Pickup2":
				return Instantiate(carPickup2, initialPosition.Key, initialPosition.Value) as GameObject;
			case "SportCoupe":
				return Instantiate(carSportCoupe, initialPosition.Key, initialPosition.Value) as GameObject;
		}
		return null;
	}

	private List<KeyValuePair<Vector3,Quaternion>> GetCarInitialPositions(int comp_id)
	{
		List<KeyValuePair<Vector3,Quaternion>> list = new List<KeyValuePair<Vector3,Quaternion>>();
		string text = new ScenarioComponent().GetComponentInitialPosition(comp_id,currentSession.Scenario.Id);
		string[] positions = text.Split('/');
		for(int i=0; i< positions.Length; i++)
		{
			list.Add(GetPosition(positions[i]));
		}
		return list;
	}

	private KeyValuePair<Vector3,Quaternion> GetPosition(string text)
	{
		//vector3/quat => x,y,z,x,y,z
		string[] info = text.Split(',');
		float vecX,vecY,vecZ,quatX,quatY,quatZ;
		vecX = float.Parse(info[0], CultureInfo.InvariantCulture.NumberFormat);
		vecY = float.Parse(info[1], CultureInfo.InvariantCulture.NumberFormat);
		vecZ = float.Parse(info[2], CultureInfo.InvariantCulture.NumberFormat);
		quatX = float.Parse(info[3], CultureInfo.InvariantCulture.NumberFormat);
		quatY = float.Parse(info[4], CultureInfo.InvariantCulture.NumberFormat);
		quatZ = float.Parse(info[5], CultureInfo.InvariantCulture.NumberFormat);
		Vector3 pos = new Vector3(vecX,vecY,vecZ);
		Quaternion quat = Quaternion.Euler(quatX,quatY,quatZ);
		
		return new KeyValuePair<Vector3, Quaternion>(pos,quat);

	}

	
}
