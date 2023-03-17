using SonaruUtilities;
using UnityEngine;

namespace Script.Other
{
    [CreateAssetMenu(fileName = "BGM Container", menuName = "FX/BGM")]
    public class BGMContainer : KeyValueScriptableObject<BGMType, AudioClip> { }
}