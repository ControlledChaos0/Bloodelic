using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Random = System.Random;


public class AudioController : MonoBehaviour
{
    private static List<Tuple<EventInstance, string, PARAMETER_ID>> eventInstances;
    
    private EventInstance eventBackgroundMusic;

    private EVENT_CALLBACK eventCallback;

    private bool gameStart;
    private bool isPaused;
    
    /*
     * Singleton Insurance
     */
    public static AudioController instance { get; private set; }
    private void Awake()
    {
        if (instance != null )
        {
            
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        eventInstances = new List<Tuple<EventInstance, string, PARAMETER_ID>>();
        // eventCallback = SFXRelease;
    }
    
    private void Start()
    {
        int index = CreateInstance(AudioEvents.backgroundMusic);
        eventInstances[index].Item1.start();
        
        EventInstance eventInstance = RuntimeManager.CreateInstance(AudioEvents.backgroundMusic);
        
        eventInstance.getDescription(out EventDescription eventDescription);
        eventDescription.getParameterDescriptionByName("IsTitle", out PARAMETER_DESCRIPTION parameterDescription);
        PARAMETER_ID parameterID = parameterDescription.id;
        eventInstances.Add(Tuple.Create(eventInstance, "BGM", parameterID));
    }
    
    private void OnDestroy()
    {
        foreach (Tuple<EventInstance, string, PARAMETER_ID> i in eventInstances)
        {
            i.Item1.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            i.Item1.release();
        }
    }
    /// <summary>
    /// Pauses all sound. If sounds are already paused, unpause.
    /// </summary>
    private void ToggleAudio()
    {
        if (!isPaused)
        {
            isPaused = true;
        }
        else
        {
            isPaused = false;
        }

        int count = 0;
        for (int i = eventInstances.Count - 1; i >= 0; i--)
        {
            if (eventInstances[i].Item1.isValid())
            {
                eventInstances[i].Item1.setPaused(isPaused);
            }
            else
            {
                eventInstances.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// Toggles Background Music from title screen to game audio.
    /// </summary>
    public void ToggleBGM()
    {
        if (!gameStart)
        {
            gameStart = true;
            eventInstances[0].Item1.setParameterByID(eventInstances[0].Item3, 0);
        }
        else
        {
            gameStart = false;
            eventInstances[0].Item1.setParameterByID(eventInstances[0].Item3, 1);
        }

    }    
    /// <summary>
    /// Plays a sound effect based on string passed in from AudioEvents.cs. Only use for sfx_* files.
    /// </summary>
    /// <param name="type">The index of the sound effect being played, obtained when creating an instance.
    /// </param>
    /// <see cref="AudioEvents"/>
    public void PlaySFX(int index, bool oneShot = true)
    {
        if (eventInstances[index].Item2 == null)
        {
            eventInstances[index].Item1.start();
        }
        else
        {
            if (eventInstances[index].Item2 == "RandomSound")
            {
                Debug.Log("RANDOM");
                eventInstances[index].Item1.getParameterByID(eventInstances[index].Item3, out float value);
                Random random = new Random();
                eventInstances[index].Item1.setParameterByID(eventInstances[index].Item3, random.Next((int)value + 1));
            }
        }
        eventInstances[index].Item1.start();
        
        /*
         * Deletes event instance if sound is a oneShot
         */
        if (oneShot)
        {
            eventInstances[index].Item1.setCallback(eventCallback, EVENT_CALLBACK_TYPE.STOPPED);
        }
    }

    private static FMOD.RESULT SFXRelease(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);
        instance.release();
        Debug.Log("CLEANED UP");
        return FMOD.RESULT.OK;
    }
    /// <summary>
    /// Creates new EventInstances, and checks if any parameters are associated with it. If so, retrieves the parameter ID.
    /// </summary>
    /// <param name="type"> The path of the sound effect. For full list of sfx, see AudioEvents.cs
    /// </param>
    /// <returns>
    /// Tuple of the EventInstance and parameter + ID associated with it.
    /// </returns>
    public int CreateInstance(string path)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(path);
        
        /*
         * Sett ing parameters by ID is more efficient, do it like this!
         */
        eventInstance.getDescription(out EventDescription eventDescription);
        eventDescription.getUserProperty("HasParameter", out USER_PROPERTY userProperty);
        string parameterName = null;
        PARAMETER_ID parameterID = new PARAMETER_ID();
        if (userProperty.floatValue().Equals(1.0f))
        {
            eventDescription.getUserProperty("RandomSound", out userProperty);
            if (userProperty.floatValue().Equals(1.0f))
            {
                parameterName = "RandomSound";
                eventDescription.getParameterDescriptionByName("Random",
                    out PARAMETER_DESCRIPTION parameterDescription);
                parameterID = parameterDescription.id;
            }
        }

        Tuple<EventInstance, string, PARAMETER_ID> newEventInstance = Tuple.Create(eventInstance, parameterName, parameterID);
        eventInstances.Add(newEventInstance);
        return eventInstances.Count - 1;
    }
}
