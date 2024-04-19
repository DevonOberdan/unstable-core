using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using ScriptableObjectLibrary;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer master;

    [SerializeField] Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    float oneShotDefaultPitch;

    const string MASTER_GROUP = "Master";
    const string MUSIC_GROUP = "Music";
    const string SFX_GROUP = "SFX";
    const string VOLUME_SUFFIX = "Volume";

    AudioSource sfxSource, musicSource;


    [SerializeField] AudioPlayEventSO sfxPlayEvent, musicPlayEvent;

    List<GameObject> soundSourcePool;
    GameObject soundSourceFactory;

    void Awake()
    {
        transform.GetChild(0).TryGetComponent(out sfxSource);
        transform.GetChild(1).TryGetComponent(out musicSource);

        oneShotDefaultPitch = sfxSource.pitch;
    }

    void OnEnable()
    {
        EventManager.AddListener<AudioSettingsChangedEvent>(evt => UpdateVolumeSettings(evt.changedSettings));
        sfxPlayEvent.OnRequestAudio += CreateSound;
        musicPlayEvent.OnRequestAudio += ChangeTrack;
    }
    void OnDisable()
    {
        EventManager.RemoveListener<AudioSettingsChangedEvent>(evt => UpdateVolumeSettings(evt.changedSettings));
        sfxPlayEvent.OnRequestAudio -= CreateSound;
        musicPlayEvent.OnRequestAudio -= ChangeTrack;
    }


    void UpdateVolumeSettings(SettingsAudio settings)
    {
        SetVolume(MASTER_GROUP + VOLUME_SUFFIX, settings.MasterVolume / 100f);
        SetVolume(MUSIC_GROUP + VOLUME_SUFFIX, settings.MusicVolume / 100f);
        SetVolume(SFX_GROUP + VOLUME_SUFFIX, settings.SfxVolume / 100f);
    }

    void SetVolume(string mixerGroup, float linearValue)
    {
        if (master == null)
            return;

        float decibelVal = GetDecibelValueFromLinear(linearValue);
        master.SetFloat(mixerGroup, decibelVal);
    }

    float GetDecibelValueFromLinear(float linearValue) => linearValue == 0 ? -144f : 20.0f * Mathf.Log10(linearValue);

    void OneShot(AudioClip clip, float newPitch, float volume = 1)
    {
        sfxSource.pitch = newPitch;
        sfxSource.PlayOneShot(clip, volume);
        StopAllCoroutines();
        StartCoroutine(ResetPitch(clip.length));
    }

    void PlaySound(AudioClip clip) => OneShot(clip, oneShotDefaultPitch);
    void PlaySoundPitched(AudioClip clip, float pitch) => OneShot(clip, pitch);
    void PlaySoundRandomPitch(AudioClip clip) => OneShot(clip, Random.Range(pitchRange.x, pitchRange.y));

    IEnumerator ResetPitch(float time)
    {
        yield return new WaitForSeconds(time+0.05f);
        if (!sfxSource.isPlaying)
            sfxSource.pitch = oneShotDefaultPitch;
    }

    void ChangeTrack(AudioConfigSO newTrack)
    {
        musicSource.Stop();
        musicSource.clip = newTrack.Clip;
        musicSource.Play();
    }

    void CreateSound(AudioConfigSO audioToPlay)
    {
        if (audioToPlay.RandomPitch)
            PlaySoundRandomPitch(audioToPlay.Clip);
        else
            PlaySound(audioToPlay.Clip);
    }
}