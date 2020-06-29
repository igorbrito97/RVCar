using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    [SerializeField] private Button buttonMessage;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void Click()
    {
        buttonMessage.GetComponent<RectTransform>().localPosition =
                        new Vector3(buttonMessage.GetComponent<RectTransform>().localPosition.x,
                        -775f, 0f);
    }
}
