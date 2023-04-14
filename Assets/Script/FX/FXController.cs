
using System;
using System.Collections.Generic;
using Script.Other;
using SonaruUtilities;
using UnityEngine;

public class FXController : TSingletonMonoBehaviour<FXController>
{
    [SerializeField] private VFXContainer vfxContainer;
    [SerializeField] private SFXContainer sfxContainer;
    [SerializeField] private BGMContainer bgmContainer;
    
    private Dictionary<VFXType, GameObject> vfxDict;
    private Dictionary<SFXType, AudioClip> sfxDict;
    private Dictionary<BGMType, AudioClip> bgmDict;

    private AudioManager AudioManager => FindObjectOfType<AudioManager>();
    
    protected override void Awake()
    {
        base.Awake();

        vfxDict = vfxContainer.GenerateDictionary();
        sfxDict = sfxContainer.GenerateDictionary();
        bgmDict = bgmContainer.GenerateDictionary();

    }


    public GameObject InitVFX(VFXType type, Vector3 position)
    {
        if (vfxDict == null || !vfxDict.ContainsKey(type)) return null;
        var vfx = Instantiate(vfxDict[type]);
        vfx.transform.position = position;

        return vfx;
    }


    public AudioClip InitSfx(SFXType type, bool untilPlayOver = false, bool stopNowPlay = false)
    {
        if (sfxDict == null || !sfxDict.ContainsKey(type)) return null;
        var sfx = sfxDict[type];
        AudioManager.SpawnSfx(sfx, untilPlayOver, stopNowPlay);
        
        return sfx;
    }
    
    
    public AudioClip InitPlayerSfx(SFXType type, int index, bool untilPlayOver = false, bool stopNowPlay = false)
    {
        if (sfxDict == null || !sfxDict.ContainsKey(type)) return null;
        var sfx = sfxDict[type];
        AudioManager.SpawnPlayerSfx(sfx, index, untilPlayOver, stopNowPlay);
        
        return sfx;
    }


    public AudioClip ChangeBGM(BGMType type, float fadeTime = 0f)
    {
        if (bgmDict == null || !bgmDict.ContainsKey(type)) return null;
        var bgm = bgmDict[type];
        AudioManager.PlayBGM(bgm, fadeTime);

        return bgm;
    }
}
