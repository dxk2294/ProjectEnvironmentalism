﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class Sounds
{
    public static class Music
    {
        public const string Test = "Music/Test";
    }
}

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject audioSourcePrefab;

    private Dictionary<string, SoundInfo> sounds = new Dictionary<string, SoundInfo>();
    private List<AudioSource> audioSources = new List<AudioSource>();

    public float GlobalVolume { get; protected set; }
    public float MusicVolume { get; protected set; }
    public float SFXVolume { get; protected set; }
    public float EnvironmentVolume { get; protected set; }

    public void RegisterSoundInfo(SoundInfo soundInfo)
    {
        sounds[soundInfo.id] = soundInfo;
    }
    
    public void Play(string id)
    {
        Debug.Log("Play " + id);
        AudioSource audioSource = null;
        for (int i = 0; i < 8; i++)
        {
            if(!audioSources[i].isPlaying)
            {
                audioSource = audioSources[i];
                break;
            } else
            {

                Debug.Log(audioSources[i] + " is playing");
            }
        }

        if(audioSource == null)
        {
            Debug.Log("Unable to play " + id + " as there are no audio sources available.");
            return;
        }

        SoundInfo soundInfo = sounds[id];
        if(soundInfo == null || soundInfo.audioClips.Length <= 0)
        {
            Debug.Log("Unable to play " + id + " as there is no sound info associated with the id.");
            return;
        }

        audioSource.clip = soundInfo.audioClips[Random.Range(0, soundInfo.audioClips.Length - 1)];
        audioSource.outputAudioMixerGroup = soundInfo.audioMixerGroup;
        audioSource.Play();
    }

    void Awake()
    {
        GameObject audioSourcesGO = new GameObject();
        audioSourcesGO.transform.name = "Audio Source Pool";
        audioSourcesGO.transform.parent = transform;
        for (int i = 0; i < 8; i++)
        {
            var go = Instantiate(audioSourcePrefab);
            go.transform.parent = audioSourcesGO.transform;
            go.transform.name = "Audio Source " + i;
            var audioSource = go.GetComponent<AudioSource>();
            audioSources.Add(audioSource);
        }
    }

    private void Start()
    {
        InitializeVolumes();
        Play(Sounds.Music.Test);
    }

    float GetVolumeFromPrefs(string volumeKey)
    {
        if (!PlayerPrefs.HasKey(volumeKey)) {
            PlayerPrefs.SetFloat(volumeKey, 1.0f);
            PlayerPrefs.Save();
            Debug.Log(volumeKey + " = 1.0f");
            return 1.0f;
        } else
        {
            Debug.Log(volumeKey + " = " + PlayerPrefs.GetFloat(volumeKey));
            return PlayerPrefs.GetFloat(volumeKey);
        }
    }

    void InitializeVolumes()
    {
        SetGlobalVol(GetVolumeFromPrefs("globalVol"));
        SetMusicVol(GetVolumeFromPrefs("musicVol"));
        SetSFXVol(GetVolumeFromPrefs("sfxVol"));
        SetEnvironmentVol(GetVolumeFromPrefs("environmentVol"));
    }

    public void SetGlobalVol(float volume)
    {
        GlobalVolume = volume;
        PlayerPrefs.SetFloat("globalVol", GlobalVolume);
        PlayerPrefs.Save();

        mixer.SetFloat("globalVol", Mathf.Log10(GlobalVolume) * 20);
        Debug.Log("Set global volume = " + volume);
    }

    public void SetMusicVol(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat("musicVol", MusicVolume);
        PlayerPrefs.Save();

        mixer.SetFloat("musicVol", Mathf.Log10(MusicVolume) * 20);
        Debug.Log("Set music volume = " + volume);
    }

    public void SetSFXVol(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat("sfxVol", SFXVolume);
        PlayerPrefs.Save();

        mixer.SetFloat("sfxVol", Mathf.Log10(SFXVolume) * 20);
        Debug.Log("Set sfx volume = " + volume);
    }

    public void SetEnvironmentVol(float volume)
    {
        EnvironmentVolume = volume;
        PlayerPrefs.SetFloat("environmentVol", EnvironmentVolume);
        PlayerPrefs.Save();

        mixer.SetFloat("environmentVol", Mathf.Log10(EnvironmentVolume) * 20);
        Debug.Log("Set environment volume = " + volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
