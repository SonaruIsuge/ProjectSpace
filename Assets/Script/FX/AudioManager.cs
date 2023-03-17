
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private const string MAIN_VOLUME = "MainVolume";
    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";

    public void SpawnSFX(AudioClip clip, bool untilPlayOver = false)
    {
        if(untilPlayOver && sfxAudioSource.isPlaying) return;
        
        sfxAudioSource.PlayOneShot(clip);
    }
    
    
    public async void PlayBGM(AudioClip clip, float fadeTime)
    {
        var timer = 0f;
        audioMixer.GetFloat(BGM_VOLUME, out var currentVolume);
        var originVolume = currentVolume;
        
        currentVolume = Mathf.Pow(10, currentVolume / 20);
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            var newVol = Mathf.Lerp(currentVolume, 0.001f, timer / fadeTime);
            audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(newVol) * 20);
            await Task.Yield();
        }

        bgmAudioSource.clip = clip;
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
