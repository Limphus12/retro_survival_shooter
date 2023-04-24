    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public enum FirearmSize { SMALL, MEDIUM, LARGE } //prolly gonna be used to determine if we can put the gun in a holster or have to lug it on our back.
    
    [Serializable]
    public enum FirearmFireType { SEMI, BURST, AUTO, COCK } //mostly gonna be using the cock and semi fire types, as we're using older weapons in this game.
    
    [Serializable]
    public enum FirearmShotType { BULLET, BUCKSHOT, EXPLOSIVE } //will mostly use bullet and buckshot shot types, as we are using older weapons in the game.

    [Serializable]
    public enum FirearmReloadType { CYLINDER, BOLT, MAGAZINE } //will mostly use cylinder and bolt reload types, as we are using older weapons in the game.

    public enum FirearmState { IDLE, ATTACKING, AIMING, AIMATTACK, RELOADING, COCKING }

    public class Firearm : MonoBehaviour
    {
        [Header("Attributes - Firearm")]
        [SerializeField] private FirearmData firearmData;

        public Magazine Magazine { get; private set; }

        #region Private Variables

        private float firearmDamage, firearmAttackRate;

        private int magazineSize;
        private float reloadTime;
        private int maxAmmoReserves;

        private FirearmFireType fireType;
        private FirearmSize size;
         
        private FirearmShotType shotType;

        private FirearmReloadType reloadType;

        private float cockTime;

        private WeaponRecoil weaponRecoil;
        private WeaponRecoil cameraRecoil;

        private FirearmSound firearmSound;

        private FirearmSway firearmSway;
        private FirearmAnimation firearmAnimation;
        private FirearmFX firearmFX;

        private FirearmFunctionAnimation firearmFunctionAnimation;
        
        private Transform playerCamera;

        private bool isAttacking, isAiming, isReloading, isCocked, isCocking;

        #endregion

        #region Initialization

        private void Awake() => Init();

        private void Init()
        {
            if (!Magazine) Magazine = GetComponent<Magazine>();
            if (!playerCamera) playerCamera = Camera.main.transform;

            InitStats(); InitEffects();
        }

        private void InitStats()
        {
            if (!firearmData) Debug.LogError("No Firearm Data Found!");

            firearmDamage = firearmData.firearmDamage;
            firearmAttackRate = firearmData.firearmAttackRate;

            magazineSize = firearmData.magazineSize;
            reloadTime = firearmData.reloadTime;
            maxAmmoReserves = firearmData.maxAmmoReserves;

            fireType = firearmData.fireType;
            size = firearmData.size;

            shotType = firearmData.shotType;
            reloadType = firearmData.reloadType;

            cockTime = firearmData.cockTime;
        }

        private void InitEffects()
        {
            if (!weaponRecoil) weaponRecoil = gameObject.GetComponentInChildren<WeaponRecoil>();

            if (!cameraRecoil && playerCamera) cameraRecoil = playerCamera.GetComponentInParent<WeaponRecoil>();

            if (!firearmSway) firearmSway = gameObject.GetComponent<FirearmSway>();

            if (!firearmAnimation) firearmAnimation = gameObject.GetComponent<FirearmAnimation>();

            if (!firearmSound) firearmSound = gameObject.GetComponent<FirearmSound>();

            if (!firearmFX) firearmFX = gameObject.GetComponent<FirearmFX>();

            if (!firearmFunctionAnimation) firearmFunctionAnimation = gameObject.GetComponent<FirearmFunctionAnimation>();
        }

        #endregion

        public bool InUse()
        {
            if (isAttacking || isReloading || isCocking) return true;

            else return false;
        }

        public void CheckInputs(bool leftMouseInput, bool rightMouseInput, bool reloadInput)
        {
            if (Magazine)
            {
                if (Magazine.IsReloading) 
                { 
                    if (leftMouseInput) Magazine.InterruptReload(); return; 
                }

                else if (!Magazine.IsReloading && !isAttacking && reloadInput)
                {
                    Magazine.CheckReload();

                    if (Magazine.IsReloading)
                    {
                        if (firearmSound) firearmSound.PlayReloadingSound();

                        Aim(false); return;
                    }
                }
            }

            if (isAttacking) return;

            Aim(rightMouseInput);

            //if we're not shooting, run through the fire types
            switch (fireType)
            {
                case FirearmFireType.SEMI:

                    if (Magazine.CurrentMagazineCount <= 0)
                    {
                        if (Input.GetMouseButtonDown(0) && firearmSound) 
                        { 
                            firearmSound.PlayDryFiringSound(); 
                        }

                        return;
                    }

                    if (leftMouseInput) StartAttack();

                    break;

                case FirearmFireType.AUTO:

                    if (Magazine.CurrentMagazineCount <= 0)
                    {
                        if (Input.GetMouseButtonDown(0) && firearmSound)
                        {
                            firearmSound.PlayDryFiringSound();
                        }

                        return;
                    }

                    if (leftMouseInput) StartAttack();

                    break;

                case FirearmFireType.COCK:

                    if (isCocking) return;

                    else if (!isCocked) { StartCock(); return; }

                    if(Magazine.CurrentMagazineCount <= 0)
                    {
                        if (Input.GetMouseButtonDown(0) && firearmSound)
                        {
                            firearmSound.PlayDryFiringSound();
                        }

                        return;
                    }

                    if (leftMouseInput && isCocked) { isCocked = false; StartAttack(); }

                    break;
            }
        }

        #region Attack Functions

        private void StartAttack()
        {
            isAttacking = true;

            if (firearmSound) firearmSound.PlayFiringSound();
            
            if (firearmFX) { firearmFX.PlayBulletEffect(); firearmFX.PlayMuzzleEffect(); }

            if (firearmFunctionAnimation) firearmFunctionAnimation.PlayFirearmUnCock();

            Attack();

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndAttack), 1 / firearmAttackRate);
        }

        private void Attack()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);

            //remove ammo from the mag
            Magazine.DepleteAmmoFromMagazine(1);

            //if we have the camera and weapon recoil references, call the recoil method on them too
            if (cameraRecoil && weaponRecoil) { cameraRecoil.Recoil(); weaponRecoil.Recoil(); }
        }

        private void EndAttack() => isAttacking = false;

        private void Hit(Transform point)
        {
            //TO DO: implement different bullet types e.g. bullet, buckshot, explosive etc.

            //raycast shooting for simple ranged combat
            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, Mathf.Infinity))
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();

                if (damageable != null) damageable.Damage(firearmDamage);
            }
        }

        #endregion

        #region Other Functions

        private void Aim(bool b)
        {
            if (Magazine.IsReloading) b = false;

            else isAiming = b;

            //if we have the camera and weapon recoil references, as well as the weapon sway reference, call the aim method on them too
            if (cameraRecoil && weaponRecoil && firearmSway)
            {
                cameraRecoil.Aim(b);
                weaponRecoil.Aim(b);
                firearmSway.Aim(b);
            }
        }

        public FirearmState GetFirearmState()
        {
            if (isCocking) return FirearmState.COCKING;
            else if (Magazine.IsReloading) return FirearmState.RELOADING;
            else if (isAiming && !isAttacking) return FirearmState.AIMING;
            if (isAttacking && isAiming) return FirearmState.AIMATTACK;
            else if (isAttacking) return FirearmState.ATTACKING;
            else return FirearmState.IDLE;
        }

        private void StartCock()
        {
            isCocking = true;

            if (firearmSway) firearmSway.Cock(isCocking);
            if (firearmSound) firearmSound.PlayCockingSound();
            if (firearmFunctionAnimation) firearmFunctionAnimation.PlayFirearmCock();

            Invoke(nameof(Cock), cockTime);
        }

        private void Cock() { isCocked = true; EndCock(); }
        private void EndCock() { isCocking = false; if (firearmSway) firearmSway.Cock(isCocking); }
        private void InterruptCock() { CancelInvoke(nameof(Cock)); EndCock(); }

        //a method to interrupt the firearm functions
        public void Interrupt()
        {
            if (!InUse()) return;

            else
            {
                if (Magazine && Magazine.IsReloading) Magazine.InterruptReload();

                if (isCocking) InterruptCock();
            }
        }

        #endregion
    }
}