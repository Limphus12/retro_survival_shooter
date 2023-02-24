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

        private FirearmFireType fireType;
        private FirearmSize size;
         
        private FirearmShotType shotType;
        private FirearmReloadType reloadType;

        private float cockTime; //hah, cock

        private FirearmSound firearmSound;

        private FirearmSway firearmSway;
        private FirearmAnimation firearmAnimation;
        
        [Space]
        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        private Transform playerCamera;

        private bool isAttacking, isAiming, isReloading, isCocked, isCocking;
        private int currentAmmo;

        private void Awake() => Init();

        private void Init()
        {
            InitStats(); InitEffects();

            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        private void InitStats()
        {
            if (!firearmData) Debug.LogError("No Firearm Data Found!");

            firearmDamage = firearmData.firearmDamage;
            firearmAttackRate = firearmData.firearmAttackRate;

            magazineSize = firearmData.magazineSize;
            reloadTime = firearmData.reloadTime;

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
        }

        public bool InUse()
        {
            if (isAttacking || isReloading || isCocking) return true;

            else return false;
        }

        public void CheckInputs(bool leftMouseInput, bool rightMouseInput, bool reloadInput)
        {
            //if we press the r key and can reload, and we're not already reloading, and we can reload, then reload!
            if (reloadInput && !isReloading && !isAttacking && (CheckReload() == 0 || CheckReload() == 1))
            {
                //TODO: need to add extra functionality i.e. single bullet at-a-time reloads

                //make sure to stop aiming lmao.
                StartReload(); Aim(false); return;
            }

            //if we're already reloading, dont do anything else here.
            else if (isReloading) return;

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
                    if (CheckReload() == 0)
                    {
                        Debug.Log("No ammo in the mag, we can't fire!");

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
                    if (CheckReload() == 0)
                    {
                        Debug.Log("No ammo in the mag, we can't fire!");

                        //if we have the firearm sound reference, call the play dry firing sound
                        if (firearmSound) firearmSound.PlayDryFiringSound();

                        return;
                    }

                    if (leftMouseInput) StartAttack();

                    break;
                case FirearmFireType.COCK:

                    //if we have no ammo tho, cry abou it
                    if (CheckReload() == 0)
                    {
                        //if we have the firearm sound reference and we press teh left mouse button
                        //*down*, call the play firing sound
                        if (Input.GetMouseButtonDown(0) && firearmSound)
                        {
                            firearmSound.PlayDryFiringSound();
                        }

                        Debug.Log("No ammo in the mag, we can't cock our weapon!");
                        return;
                    }

                    //if we're cocking
                    if (isCocking)
                    {
                        Debug.Log("Cannot Fire! We are Cocking the Weapon!");
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

        //starts shooting
        private void StartAttack()
        {
            isAttacking = true;

            //shoot immedietly, as this is a gun
            Attack();

            //if we have the firearm sound reference, call the play firing sound
            if (firearmSound) firearmSound.PlayFiringSound();

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndAttack), 1 / firearmAttackRate);
        }

        //shoots!
        private void Attack()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);

            //i think i wanna add an ammo usage variable in teh future, but idk yet.
            currentAmmo -= 1;

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
            isAiming = b;

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
            else if (isReloading) return FirearmState.RELOADING;
            else if (isAiming && !isAttacking) return FirearmState.AIMING;
            if (isAttacking && isAiming) return FirearmState.AIMATTACK;
            else if (isAttacking) return FirearmState.ATTACKING;
            else return FirearmState.IDLE;
        }

        private int CheckReload()
        {
            if (currentAmmo <= 0)
            {
                //Debug.Log("Cannot Shoot, we have no ammo!");

                return 0; //if we have no ammo in the mag
            }

            else if (currentAmmo < magazineSize)
            {
                //Debug.Log("We have ammo, but not a full mag!");

                return 1; //if we dont have the full amount
            }

            else if (currentAmmo == magazineSize)
            {
                //Debug.Log("We got a full mag, let 'em loose!");

                return 2; //if we are full
            }

            else return 3;
        }

        //starts reloading
        private void StartReload()
        {
            Debug.Log("Reloading!");

            //set isreloading to true and invoke our reload method
            isReloading = true;

            //if we have the weapon sway reference, call the reload method on it too
            if (firearmSway) firearmSway.Reload(isReloading);

            //if we have the firearm sound reference, call the play reload sound
            if (firearmSound) firearmSound.PlayReloadingSound();

            Invoke(nameof(Reload), reloadTime);
        }

        private void Reload()
        {
            //set our current ammo equal to our magazine size, and end the reload
            currentAmmo = magazineSize; EndReload();
        }

        //ends reloading
        private void EndReload()
        {
            Debug.Log("Done our Reload!");

            isReloading = false;

            //if we have the weapon sway reference, call the reload method on it too
            if (firearmSway) firearmSway.Reload(isReloading);
        }

        private void InterruptReload()
        {
            CancelInvoke(nameof(Reload));

            EndReload();
        }

        //starts cocking
        private void StartCock()
        {
            Debug.Log("Cocking!");

            //set isCocking to true and invoke our cock method
            isCocking = true;

            //if we have the weapon sway reference, call the cock method on it too
            if (firearmSway) firearmSway.Cock(isCocking);

            //if we have the firearm sound reference, call the play cocking sound
            if (firearmSound) firearmSound.PlayCockingSound();

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
            Debug.Log("Cocked!");

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
                if (isReloading) InterruptReload();

                if (isCocking) InterruptCock();
            }
        }
    }
}