using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "FirearmSoundData", menuName = "Sounds/Firearm")]
    public class FirearmSoundData : ItemSoundData
    {
        [Header("Attributes - Firearm")]
        public AudioClip firingClip;
        public AudioClip dryFiringClip, reloadingClip;

        [Space]
        [Tooltip("Only Used for the BOLT FireType")]
        public AudioClip cockingClip;
    }
}