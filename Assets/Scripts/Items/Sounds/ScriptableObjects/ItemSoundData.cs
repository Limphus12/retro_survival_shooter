using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "ItemSoundData", menuName = "Sounds/Item")]
    public class ItemSoundData : ScriptableObject
    {
        [Header("Attributes - Item")]
        public AudioClip equipClip;

        public AudioMixerGroup audioMixerGroup;
    }
}