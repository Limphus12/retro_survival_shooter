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

        private float firearmDamage, firearmAttackRate;

        private int magazineSize;
        private float reloadTime;
        private int maxAmmoReserves;

        private FirearmFireType fireType;
        private FirearmSize size;
         
        private FirearmShotType shotType;

        [Header("Attributes - Reloading")]
        [SerializeField] private bool infiniteAmmo;

        public Magazine Magazine { get; private set; }

        private FirearmReloadType reloadType;

        private float cockTime; //hah, cock

        private FirearmSound firearmSound;

        private FirearmSway firearmSway;
        private FirearmAnimation firearmAnimation;
        private FirearmFX firearmFX;

        private FirearmFunctionAnimation firearmFunctionAnimation;
        
        [Space]
        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        private Transform playerCamera;

        private bool isAttacking, isAiming, isReloading, isCocked, isCocking;

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
            if (!firearmSway) firearmSway = gameObject.GetComponent<FirearmSway>();

            if (!firearmAnimation) firearmAnimation = gameObject.GetComponent<FirearmAnimation>();

            if (!firearmSound) firearmSound = gameObject.GetComponent<FirearmSound>();

            if (!firearmFX) firearmFX = gameObject.GetComponent<FirearmFX>();

            if (!firearmFunctionAnimation) firearmFunctionAnimation = gameObject.GetComponent<FirearmFunctionAnimation>();
        }

        public bool InUse()
        {
            if (isAttacking || isReloading || isCocking) return true;

            else return false;
        }

        public void CheckInputs(bool leftMouseInput, bool rightMouseInput, bool reloadInput)
        {
            if (infiniteAmmo)
            {
                //if were attacking already, dont do anything
                if (isAttacking) return;

                //check for r-mouse input and update aiming
                Aim(rightMouseInput);

                //if we're not shooting, run through the fire types
                switch (fireType)
                {
                    case FirearmFireType.SEMI:

                        //TO DO; actually implement semi auto fire e.g. the player has to click the mouse to fire,
                        //not just hold it down. (maybe ill just get rid of semi and just use auto, but with a low fire rate?)

                        if (leftMouseInput) StartAttack();

                        break;

                    case FirearmFireType.BURST:

                        //TO DO; implement burst firing (i have done so in the past, so it should be easy)

                        break;
                    case FirearmFireType.AUTO:

                        if (leftMouseInput) StartAttack();

                        break;
                    case FirearmFireType.COCK:

                        //if we're cocking
                        if (isCocking)
                        {
                            return;
                        }

                        //if we'te havent cocked, start doing so!
                        else if (!isCocked)
                        {
                            StartCock();
                            return;
                        }

                        //if were; not shooting, not cocking and we have cocked the firearm,
                        //and we press the l-mouse button
                        //uncock the weapon and start firing!
                        if (leftMouseInput && isCocked)
                        {
                            isCocked = false;

                            StartAttack();
                        }

                        break;
                }
            }

            else
            {
                if (Magazine)
                {
                    if (Magazine.IsReloading)
                    {
                        if (leftMouseInput) Magazine.InterruptReload();
                    }

                    //if we press the r key and can reload, and we're not already reloading, and we can reload, then reload!
                    else if (reloadInput && !Magazine.IsReloading && !isAttacking)
                    {
                        if (firearmSound) firearmSound.PlayReloadingSound();

                        Magazine.StartClipReload(); Aim(false); return;

                        //if we can do the clip reload
                        if (Magazine.CheckReload() == 2)
                        {
                            Magazine.StartClipReload(); Aim(false); return;
                        }

                        //if we can only do the single bullet reload
                        else if (Magazine.CheckReload() == 1)
                        {
                            Magazine.StartReload(); Aim(false); return;
                        }

                        else if (Magazine.CheckReload() == 0) { }
                    }
                }

                //if were attacking already, dont do anything
                if (isAttacking) return;

                //check for r-mouse input and update aiming
                Aim(rightMouseInput);

                //if we're not shooting, run through the fire types
                switch (fireType)
                {
                    case FirearmFireType.SEMI:

                        //TO DO; actually implement semi auto fire e.g. the player has to click the mouse to fire,
                        //not just hold it down. (maybe ill just get rid of semi and just use auto, but with a low fire rate?)

                        //if we have no ammo tho, cry about it
                        if (Magazine.CheckMagazine() == 0)
                        {
                            //if we have the firearm sound reference, call the play dry firing sound
                            if (firearmSound) firearmSound.PlayDryFiringSound();

                            return;
                        }

                        if (leftMouseInput) StartAttack();

                        break;

                    case FirearmFireType.BURST:

                        //TO DO; implement burst firing (i have done so in the past, so it should be easy)

                        break;
                    case FirearmFireType.AUTO:

                        //if we have no ammo tho, cry about it
                        if (Magazine.CheckMagazine() == 0)
                        {
                            //if we have the firearm sound reference, call the play dry firing sound
                            if (firearmSound) firearmSound.PlayDryFiringSound();

                            return;
                        }

                        if (leftMouseInput) StartAttack();

                        break;
                    case FirearmFireType.COCK:

                        //if we have no ammo tho, cry abou it
                        if (Magazine.CheckMagazine() == 0)
                        {
                            //if we have the firearm sound reference and we press teh left mouse button
                            //*down*, call the play firing sound
                            if (Input.GetMouseButtonDown(0) && firearmSound)
                            {
                                firearmSound.PlayDryFiringSound();
                            }

                            return;
                        }

                        //if we're cocking
                        if (isCocking)
                        {
                            return;
                        }

                        //if we'te havent cocked, start doing so!
                        else if (!isCocked)
                        {
                            StartCock();
                            return;
                        }

                        //if were; not shooting, not cocking and we have cocked the firearm,
                        //and we press the l-mouse button
                        //uncock the weapon and start firing!
                        if (leftMouseInput && isCocked)
                        {
                            isCocked = false;

                            StartAttack();
                        }

                        break;
                }
            }
        }

        //starts shooting
        private void StartAttack()
        {
            isAttacking = true;

            //shoot immedietly, as this is a gun
            Attack();

            //if we have the firearm sound reference, call the play firing sound
            if (firearmSound) firearmSound.PlayFiringSound();
            
            //if we have the firearm fx reference, play the bullet and muzzle effects
            if (firearmFX)
            {
                firearmFX.PlayBulletEffect();
                firearmFX.PlayMuzzleEffect();
            }

            //if we have the firearm functions reference
            if (firearmFunctionAnimation) firearmFunctionAnimation.PlayFirearmUnCock();


            //invoke end shoot after our rate of fire
            Invoke(nameof(EndAttack), 1 / firearmAttackRate);
        }

        //shoots!
        private void Attack()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);

            //i think i wanna add an ammo usage variable in teh future, but idk yet.
            Magazine.UseAmmo(1);

            //if we have the camera and weapon recoil references, call the recoil method on them too
            if (cameraRecoil && weaponRecoil)
            {
                cameraRecoil.Recoil();
                weaponRecoil.Recoil();
            }
        }

        //ends shooting
        private void EndAttack()
        {
            isAttacking = false;
        }

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

            //else if (!infiniteAmmo) { }

            else if (Magazine.IsReloading) return FirearmState.RELOADING;
            else if (isAiming && !isAttacking) return FirearmState.AIMING;
            if (isAttacking && isAiming) return FirearmState.AIMATTACK;
            else if (isAttacking) return FirearmState.ATTACKING;
            else return FirearmState.IDLE;
        }

        //starts cocking
        private void StartCock()
        {
            //set isCocking to true and invoke our cock method
            isCocking = true;

            //if we have the weapon sway reference, call the cock method on it too
            if (firearmSway) firearmSway.Cock(isCocking);

            //if we have the firearm sound reference, call the play cocking sound
            if (firearmSound) firearmSound.PlayCockingSound();

            //if we have the firearm functions reference
            if (firearmFunctionAnimation) firearmFunctionAnimation.PlayFirearmCock();

            Invoke(nameof(Cock), cockTime);
        }

        private void Cock()
        {
            //cock our gun!
            isCocked = true; EndCock();
        }

        //ends cocking
        private void EndCock()
        {
            //set isCocking to false
            isCocking = false;

            //if we have the weapon sway reference, call the cock method on it too
            if (firearmSway) firearmSway.Cock(isCocking);
        }

        //heh
        private void InterruptCock()
        {
            CancelInvoke(nameof(Cock));

            EndCock();
        }

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
    }
}