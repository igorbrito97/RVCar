using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    
    [SerializeField] private GameObject videoPlayer;
    void Start()
    {
        videoPlayer.SetActive(true);
    }

    public void onChange(){
        //bool flag = videoPlayer.activeInHierarchy;
        //videoPlayer.SetActive(!flag);
        videoPlayer.SetActive(false);
    }

}
