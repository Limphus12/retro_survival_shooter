using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    [CreateAssetMenu(fileName = "SustenanceSoundData", menuName = "Sounds/Sustenance")]
    public class SustenanceSoundData : ItemSoundData
    {
        [Header("Attributes - Sustenance")]
        public AudioClip consumeClip;
        public AudioClip consumingClip;
    }
}