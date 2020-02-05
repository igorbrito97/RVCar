//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class RespawnCenario : MonoBehaviour 
//{
//	public GameObject Cenario1,Cenario2,Cenario3;
//
//	public float SpawnRate=2f;
//	float ProximoSpawn=0;
//	int CenarioSpawn=0;
//
//	void Start () 
//	{
//		
//	}
//	
//
//	void Update () 
//	{
////		if (Time.time > ProximoSpawn) 
////		{
////
////
////			CenarioSpawn = Random.Range (1, 3);
////			Debug.Log (CenarioSpawn);
////
////			switch (CenarioSpawn) 
////			{
////
////			case 1: 
////				Instantiate (Cenario1, transform.position, Quaternion.identity);
////				break;
////			case 2:
////				Instantiate (Cenario2, transform.position, Quaternion.identity);
////				break;
////			
////			}
////
////		}
//
//		
//	}
//	void OnTriggerEnter(Collider outro)
//	{
//
//		if(outro.gameObject.tag==("Player"))
//		{
//
//			CenarioSpawn = Random.Range (1, 3);
//			Debug.Log (CenarioSpawn);
//			
//			switch (CenarioSpawn) 
//				{
//			
//					case 1: 
//				Instantiate (Cenario1,transform.position + transform.forward * 29f, transform.rotation);
//							break;
//					case 2:
//				Instantiate (Cenario2, transform.position + transform.forward * 29f, transform.rotation);
//							break;
//					case 3:
//				Instantiate (Cenario3, transform.position + transform.forward * 29f, transform.rotation);
//							break;
//						
//				}
//					Destroy (gameObject);
//
//		}
//
//	}
//}
//	
