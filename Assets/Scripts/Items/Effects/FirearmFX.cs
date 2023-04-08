using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.limphus.retro_survival_shooter
{
    public class FirearmFX : ItemFX
    {
        [SerializeField] private Transform firePoint;

        [Space]
        [SerializeField] private GameObject firearmBullet;
        [SerializeField] private float firearmBulletTime;

        [Space]
        [SerializeField] private GameObject firearmMuzzle;
        [SerializeField] private float firearmMuzzleTime;

        public void PlayBulletEffect() => PlayFX(firearmBullet, firePoint, firearmBulletTime);
        public void PlayMuzzleEffect() => PlayFX(firearmMuzzle, firePoint, firearmMuzzleTime);

    }
}