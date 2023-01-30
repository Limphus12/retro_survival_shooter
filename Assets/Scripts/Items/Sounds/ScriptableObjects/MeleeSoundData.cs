using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "MeleeSoundData", menuName = "Sounds/Melee")]
    public class MeleeSoundData : ItemSoundData
    {
        [Header("Attributes - Melee")]
        public AudioClip lightAttackClip;
        public AudioClip heavyAttackClip, exhaustedAttackClip;

        [Space]
        public AudioClip lightAttackHitClip;
        public AudioClip heavyAttackHitClip, exhaustedAttackHitClip;

        [Space]
        public AudioClip blockHitClip;

    }
}