using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "FirearmData", menuName = "Items/Firearm")]

    [System.Serializable]
    public class FirearmData : ItemData
    {
        [Header("Attributes - Firearm")]
        public float firearmDamage;
        public float firearmAttackRate;

        [Space]
        public int magazineSize;
        public float reloadTime;
        public int maxAmmoReserves;

        [Space]
        public FirearmFireType fireType;
        public FirearmSize size;

        [Space]
        public FirearmShotType shotType;
        public FirearmReloadType reloadType;

        [Space]
        [Tooltip("Only used for the BOLT Fire Type")]
        public float cockTime; //hah again, cock lmao
    }
}