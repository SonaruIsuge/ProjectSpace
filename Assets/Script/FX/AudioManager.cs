
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private List<AudioSource> playerAudioSources;


    private const string MAIN_VOLUME = "MainVolume";
    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";


    public void SpawnSfx(AudioClip clip, bool untilPlayOver = false, bool stopNowPlay = false)
    {
        if(untilPlayOver && sfxAudioSource.isPlaying) return;
    
        if (stopNowPlay && sfxAudioSource.isPlaying) sfxAudioSource.Stop();
        sfxAudioSource.PlayOneShot(clip);
    }
    
    
    public void SpawnPlayerSfx(AudioClip clip, int index, bool untilPlayOver = false, bool stopNowPlay = false)
    {
        if(index < 0 || index >= playerAudioSources.Count) return;
        
        if(untilPlayOver && playerAudioSources[index].isPlaying) return;
    
        if (stopNowPlay && playerAudioSources[index].isPlaying) playerAudioSources[index].Stop();
        playerAudioSources[index].PlayOneShot(clip);
    }


    public void StopPlayerSfx(int index)
    {
        if(index < 0 || index >= playerAudioSources.Count) return;
        if(!playerAudioSources[index].clip) return;
        playerAudioSources[index].clip = null;
    }


    public void StopSfx()
    {
        if(!sfxAudioSource.clip) return;
        sfxAudioSource.Stop();
        sfxAudioSource.clip = null;
    }
    
    
    public async void PlayBGM(AudioClip clip, float fadeTime)
    {
        var timer = 0f;
        audioMixer.GetFloat(BGM_VOLUME, out var currentVolume);
        currentVolume = Mathf.Pow(10, currentVolume / 20);
        var originVolume = currentVolume;
        
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            var newVol = Mathf.Lerp(currentVolume, 0.001f, timer / fadeTime);
            audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(newVol) * 20);
            await Task.Yield();
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
        
        timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            var newVol = Mathf.Lerp(currentVolume, originVolume, timer / fadeTime);
            audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(newVol) * 20);
            await Task.Yield();
        }
    }
}
