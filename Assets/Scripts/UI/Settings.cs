using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Turn this into abstract class
/*
 * Virtual methods:
 * 
 *  Load()
 *  Save()
 *  Reset()
 * 
 * AudioSettings.cs will extend from it
 * etc, etc.
 * 
 * 
 */

[System.Serializable]
public abstract class Settings
{
    //const float DEFAULT_VALUE = 80f;

    //float masterVolume;
    //float musicVolume;
    //float sfxVolume;

    //public float MasterVolume { 
    //    get => masterVolume; 
    //    set 
    //    {
    //        masterVolume = value;
    //        Broadcast(this);
    //    }
    //}

    //public float MusicVolume {
    //    get => musicVolume;
    //    set {
    //        musicVolume = value;
    //        Broadcast(this);
    //    }
    //}

    //public float SfxVolume {
    //    get => sfxVolume;
    //    set {
    //        sfxVolume = value;
    //        Broadcast(this);
    //    }
    //}

    //protected virtual void Broadcast(Settings changedSettings)
    //{
    //    Events.onSettingsChanged.changedSettings = changedSettings;
    //    EventManager.Broadcast(Events.onSettingsChanged);
    //}

    public abstract void Load();

    public virtual void Reset()
    {
        Save();
    }

    public virtual void Save()
    {
        PlayerPrefs.Save();
    }
}
