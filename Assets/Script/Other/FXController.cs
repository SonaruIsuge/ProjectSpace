
using System;
using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;

public class FXController : TSingletonMonoBehaviour<FXController>
{
    [SerializeField] private VFXContainer vfxContainer;

    private Dictionary<VFXType, GameObject> vfxDict;

    
    protected override void Awake()
    {
        base.Awake();

        vfxDict = vfxContainer.GenerateDictionary();

    }


    public GameObject InitVFX(VFXType type, Vector3 position)
    {
        if (vfxDict == null || !vfxDict.ContainsKey(type)) return null;
        var vfx = Instantiate(vfxDict[type]);
        vfx.transform.position = position;

        return vfx;
    }
}
