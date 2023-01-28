using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public enum FirearmSize { SMALL, LARGE } //prolly gonna be used to determine if we can put the gun in a holster or have to lug it on our back.
    
    [Serializable]
    public enum FirearmFireType { SEMI, BURST, AUTO, COCK } //mostly gonna be using the bolt and semi fire types, as we're using older weapons in this game.

    [Serializable]
    public enum FirearmShotType { BULLET, BUCKSHOT, EXPLOSIVE } //will mostly use bullet and buckshot shot types, as we are using older weapons in the game.

    [Serializable]
    public enum FirearmReloadType { CYLINDER, BOLT, MAGAZINE } //will mostly use cylinder and bolt reload types, as we are using older weapons in the game.

    public class Firearm : Weapon
    {
        [Header("Attributes - Firearm")]
        [SerializeField] private FirearmData firearmData;

        [Space]
        [SerializeField] private int magazineSize;
        [SerializeField] private float reloadTime;

        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        [Space]
        [SerializeField] private FirearmSway firearmSway;

        [Space]
        [SerializeField] private FirearmFireType fireType;
        [SerializeField] private FirearmSize size;

        [Space]
        [SerializeField] private FirearmShotType shotType;
        [SerializeField] private FirearmReloadType reloadType;

        [Space]
        [SerializeField] private float cockTime; //hah, cock

        [Space]
        [SerializeField] private FirearmSound firearmSound;
        [SerializeField] private FirearmAnimation firearmAnimation;

        private bool isAiming, isReloading, reloadInput, isCocked, isCocking;
        private int currentAmmo;

        //initialization
        protected override void Init()
        {
            if (!firearmData)
            {
                Debug.LogWarning("No Firearm Data found for " + gameObject.name + "; Assign Firearm Data!");
                return;
            }

            itemName = firearmData.itemName;
            itemWeight = firearmData.itemWeight;

            damage = firearmData.damage;
            rateOfFire = firearmData.rateOfFire;

            magazineSize = firearmData.magazineSize;
            reloadTime = firearmData.reloadTime;

            fireType = firearmData.fireType;
            size = firearmData.size;

            shotType = firearmData.shotType;
            reloadType = firearmData.reloadType;

            cockTime = firearmData.cockTime;
        }

        private void Start()
        {
            //need to check if we we're doing things i.e. reloading, cocking etc.

            if (isReloading) StartReload();

            else if (isCocking || !isCocked) StartCock();
        }

        private void Update()
        {
            Inputs(); Animation();
        }

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            if (Input.GetKeyDown(KeyCode.R)) reloadInput = true;
            else if (Input.GetKeyUp(KeyCode.R)) reloadInput = false;

            CheckShoot();
        }

        protected override void CheckShoot()
        {
            //if we press the r key and can reload, and we're not already reloading, and we can reload, then reload!
            if (reloadInput && !isReloading && !isShooting && (CheckReload() == 0 || CheckReload() == 1))
            {
                //TODO: need to add extra functionality i.e. single bullet at-a-time reloads

                //make sure to stop aiming lmao.
                StartReload(); Aim(false); return;
            }

            //if we're already reloading, dont do anything else here.
            else if (isReloading) return;

            //if were shooting already, dont do anything
            if (isShooting) return;

            //check for r-mouse input and update aiming
            Aim(rightMouseInput);

            //if we're not shooting, run through the fire types
            switch (fireType)
            {
                case FirearmFireType.SEMI:

                    //TO DO; actually implement semi auto fire e.g. the player has to click the mouse to fire,
                    //not just hold it down. (maybe ill just get rid of semi and just use auto, but with a low fire rate?)

                    //if were not shooting and we press the l-mouse button, start shooting
                    if (leftMouseInput)
                    {
                        //if we have no ammo tho, cry about it
                        if (CheckReload() == 0)
                        {
                            Debug.Log("No ammo in the mag, we can't fire!");

                            //if we have the firearm sound reference, call the play dry firing sound
                            if (firearmSound) firearmSound.PlayDryFiringSound();

                            return;
                        }

                        StartShoot();
                    }

                    break;

                case FirearmFireType.BURST:

                    //TO DO; implement burst firing (i have done so in the past, so it should be easy)

                    break;
                case FirearmFireType.AUTO:

                    //if were not shooting and we press the l-mouse button, start shooting
                    if (leftMouseInput)
                    {
                        //if we have no ammo tho, cry about it
                        if (CheckReload() == 0)
                        {
                            Debug.Log("No ammo in the mag, we can't fire!");

                            //if we have the firearm sound reference, call the play dry firing sound
                            if (firearmSound) firearmSound.PlayDryFiringSound();

                            return;
                        }

                        StartShoot();
                    }

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

                        StartShoot();
                    }
                    
                    break;
            }
        }

        //starts shooting
        protected override void StartShoot()
        {
            isShooting = true;

            //shoot immedietly, as this is a gun
            Shoot();

            //if we have the firearm sound reference, call the play firing sound
            if (firearmSound) firearmSound.PlayFiringSound();

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndShoot), 1 / rateOfFire);
        }

        //shoots!
        protected override void Shoot()
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
        protected override void EndShoot() => isShooting = false;

        protected override void Hit(Transform point)
        {
            //TO DO: implement different bullet types e.g. bullet, buckshot, explosive etc.

            //raycast shooting for simple ranged combat
            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, Mathf.Infinity))
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();

                if (damageable != null) damageable.Damage(damage);
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

        private void Animation()
        {
            //if we have the animation reference
            if (firearmAnimation)
            {
                //if we're cocking the gun, then play this anim
                if (isCocking)
                {
                    firearmAnimation.PlayFirearmCock();
                    return;
                }

                //if we're reloading the gun, then play this anim
                else if (isReloading) 
                {
                    firearmAnimation.PlayFirearmReload();
                    return;
                }

                //if we're aiming the gun and we're not shooting, play this anim
                else if (isAiming && !isShooting) 
                {
                    firearmAnimation.PlayFirearmAim();
                    return;
                }

                //if we're aiming the gun and we're shooting, play this anim
                else if (isAiming && isShooting) 
                {
                    firearmAnimation.PlayFirearmAimFire();
                    return;
                }

                //if we're shooting, play this anim
                else if (isShooting) 
                {
                    firearmAnimation.PlayFirearmFire();
                    return;
                }

                //if we're not shooting, play this anim
                else if (!isShooting) 
                {
                    firearmAnimation.PlayFirearmIdle();
                    return;
                }
            }
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

        public override ItemData GetItemData()
        {
            if (firearmData != null) return firearmData;

            else return null;
        }

        public override void SetItemData(ItemData itemData)
        {
            firearmData = (FirearmData)itemData;

            Init();
        }
    }
}