using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ContainerSound : SoundHandler
    {
        [Header("Attributes - Container")]
        [SerializeField] protected AudioClip lootClip;
        [SerializeField] protected AudioClip lootStopClip;

        public void PlayLootingSound() => PlaySound(lootClip);
        public void PlayLootingStopSound() => PlaySound(lootStopClip);
    }
}