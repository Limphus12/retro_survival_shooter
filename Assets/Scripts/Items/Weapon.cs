using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public abstract class Weapon : Item
    {
        [Header("Attributes - Weapon")]
        [SerializeField] protected WeaponData weaponData;

        [Space]
        [SerializeField] protected int damage;
        [SerializeField] protected float attackRate;

        [Space]
        [SerializeField] protected Transform playerCamera;

        protected bool isAttacking, leftMouseInput, rightMouseInput;

        protected abstract void Inputs();
        protected abstract void CheckAttack();
        protected abstract void StartAttack();
        protected abstract void Attack();
        protected abstract void EndAttack();
        protected abstract void Hit(Transform point);
    }
}