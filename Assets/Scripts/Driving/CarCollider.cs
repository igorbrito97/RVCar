using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour
{
    void Start()
    {
        
		Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Environment") || collision.collider.CompareTag("Car"))
        {
            ContactPoint point = collision.GetContact(0);
            float angle = Vector3.Angle(transform.InverseTransformPoint(point.point), this.transform.position);
            //Debug.Log(position.x + " / " + position.y + " / " + position.z + " == " + angle);
            // ver qual a diferença de uma batida na frente pra uma batida do lado - testar com outro carro
            //colocar uma tag pro carro
            if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                if(angle >= 150f && angle <= 175f) //frontal
                {
                    LogitechGSDK.LogiPlayFrontalCollisionForce(0, 60);
                }
                else if(angle < 150 && angle >= 50)// side
                {
                    LogitechGSDK.LogiPlaySideCollisionForce(0, 60);
                }
            }
        }
        else 
        {
            //lombada - nivel do chão - mudar intensidade
            
        }
    } 
}
