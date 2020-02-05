using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeradorLevel : MonoBehaviour {

	public Transform blocoCenario1;
	bool jogarFrente =false;
	//public Transform player;
	//public GameObject[] PrefabsTerrenos;
	//public Transform PlayerTransform;
	//public float CriarZ=0.0f;
	//public float Dist= 71.0f;
	//private int NumeroTerrenos =10;



	void Start () 
	{
		if (jogarFrente == true) 
		{
			Vector3 temp = transform.position;
			temp.z = 10;
			transform.position = temp;

		}



	}

	void Update () 
	{
		
	}




void OnTriggerEnter(Collider outro)
	{

		if(outro.gameObject.tag==("Player"))
			{

			Instantiate (blocoCenario1, transform.position + transform.forward * 2188f, transform.rotation);
            Debug.Log("AAAAAAAAAAAAAAAAAA");

			}

	}
}


