using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "FirearmData", menuName = "Items/Weapon/Firearm")]

    [System.Serializable]
    public class FirearmData : WeaponData
    {
        [Header("Attributes - Firearm")]
        public int magazineSize;
        public float reloadTime;

        [Space]
        public FirearmFireType fireType;
        public FirearmSize size;
    }
}