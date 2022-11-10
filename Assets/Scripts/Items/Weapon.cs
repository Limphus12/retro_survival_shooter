using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Weapon : Item
    {
        [Header("Attributes - Weapon")]
        [SerializeField] protected double damage;
        [SerializeField] protected double rateOfFire;

        [Space]
        [SerializeField] protected Transform playerCamera;

        protected bool isShooting, leftMouseInput, rightMouseInput;

        protected abstract void Inputs();
        protected abstract void CheckShoot();
        protected abstract void StartShoot();
        protected abstract void Shoot();
        protected abstract void EndShoot();
        protected abstract void Hit(Transform point);
    }
}