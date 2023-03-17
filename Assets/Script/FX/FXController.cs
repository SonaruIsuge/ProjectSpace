
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

    private AudioManager audioManager;
    
    protected override void Awake()
    {
        base.Awake();

        vfxDict = vfxContainer.GenerateDictionary();
        sfxDict = sfxContainer.GenerateDictionary();
        bgmDict = bgmContainer.GenerateDictionary();

        audioManager = FindObjectOfType<AudioManager>();
    }


    public GameObject InitVFX(VFXType type, Vector3 position)
    {
        if (vfxDict == null || !vfxDict.ContainsKey(type)) return null;
        var vfx = Instantiate(vfxDict[type]);
        vfx.transform.position = position;

        return vfx;
    }


    public AudioClip InitSFX(SFXType type)
    {
        if (sfxDict == null || !sfxDict.ContainsKey(type)) return null;
        var sfx = sfxDict[type];
        audioManager.SpawnSFX(sfx);
        
        return sfx;
    }


    public AudioClip ChangeBGM(BGMType type, float fadeTime = 0.5f)
    {
        if (bgmDict == null || !bgmDict.ContainsKey(type)) return null;
        var bgm = bgmDict[type];
        audioManager.PlayBGM(bgm, fadeTime);

        return bgm;
    }
}
