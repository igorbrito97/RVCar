using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarAudio : MonoBehaviour
{
    // This script reads some of the car's current properties and plays sounds accordingly.
    // The engine sound can be a simple single clip which is looped and pitched, or it
    // can be a crossfaded blend of four clips which represent the timbre of the engine
    // at different RPM and Throttle state.

    // the engine clips should all be a steady pitch, not rising or falling.

    // when using four channel engine crossfading, the four clips should be:
    // lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
    // highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
    // lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
    // highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)

    // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.aa

    [SerializeField] private AudioClip lowAccelClip;                                              // Audio clip for low acceleration
    [SerializeField] private AudioClip lowDecelClip;                                              // Audio clip for low deceleration
    [SerializeField] private AudioClip highAccelClip;                                             // Audio clip for high acceleration
    [SerializeField] private AudioClip highDecelClip;                                             // Audio clip for high deceleration
    [SerializeField] private AudioClip startEngineClip;
    public MainCarController manualCarController;
    public CarControllerAutomatic automaticCarController;
    public float pitchMultiplier = 1f;                                          // Used for altering the pitch of audio clips
    public float lowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
    public float lowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
    public float highPitchMultiplier = 0.25f;                                   // Used for altering the pitch of high sounds
    public float maxRolloffDistance = 500;                                      // The maximum distance where rollof starts to take place
    public float dopplerLevel = 1;                                              // The mount of doppler effect used in the audio
    public bool useDoppler = true;                                              // Toggle for using doppler
    public Camera cam;

    private AudioSource m_LowAccel; // Source for the low acceleration sounds
    private AudioSource m_LowDecel; // Source for the low deceleration sounds
    private AudioSource m_HighAccel; // Source for the high acceleration sounds
    private AudioSource m_HighDecel; // Source for the high deceleration sounds
    private AudioSource engineStart; // Source for the high deceleration sounds
    private bool isManual = false;
    private bool engineStarted = false;
    private bool engineSoundStarted = false;
    
    private void Start()
    {
        if(TryGetComponent(out manualCarController) && manualCarController.enabled)
            isManual = true;
        if(!isManual)
            automaticCarController = GetComponent<CarControllerAutomatic>();

        m_HighAccel = SetUpEngineAudioSource(highAccelClip);
        m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
        m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
        m_HighDecel = SetUpEngineAudioSource(highDecelClip);
    }

    private void StopSound()
    {
        //Destroy all audio sources on this object:
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }
    }


    // Update is called once per frame
    private void Update()
    {
        Debug.Log(engineStarted);
        Debug.Log("MANUAL: " + isManual);
        if(LogitechGSDK.LogiButtonIsPressed(0, 10) && !engineStarted && !engineSoundStarted) //start button
        {
            engineStart = gameObject.AddComponent<AudioSource>();
            engineStart.clip = startEngineClip;
            engineStart.volume = 1;
            engineStart.loop = false;
            engineStart.Play();
            engineSoundStarted = true;
        } 

        if(engineSoundStarted && !engineStart.isPlaying) //terminou de tocar
        {
            m_HighAccel.Play();
            m_HighDecel.Play();
            m_LowAccel.Play();
            m_LowDecel.Play();
            m_HighAccel.volume = 1f;
            m_LowAccel.volume = 1f;
            m_LowDecel.volume = 1f;
            m_HighDecel.volume = 1f;
            engineStarted = true;
            engineSoundStarted = false;
        }

        if(engineStarted && isManual)
        {
            //setar audio manual
            float pitch = manualCarController.currentRpm / 8000; //maxRPM
            m_HighAccel.pitch = pitch;
            m_LowAccel.pitch = pitch;
            m_LowDecel.pitch = pitch;
            m_HighDecel.pitch = pitch;
        }
        else if(engineStarted)
        {
            //setar o audio para automatico - com speed
            float pitch = automaticCarController.currentSpeed / automaticCarController.maxSpeed;
            m_HighAccel.pitch = pitch;
            m_LowAccel.pitch = pitch;
            m_LowDecel.pitch = pitch;
            m_HighDecel.pitch = pitch;
        }
    }


    // sets up and adds new audio source to the gane object
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;
        source.playOnAwake = false;
        return source;
    }
}
