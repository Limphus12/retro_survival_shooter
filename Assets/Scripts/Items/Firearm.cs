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

    public class Firearm : Melee
    {
        private FirearmData firearmData;

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

        private bool isAiming, isReloading, reloadInput, isCocked, isCocking, firearmAttack;
        private int currentAmmo;

        //initialization
        protected override void Init()
        {
            InitStats(); InitEffects();

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        protected override void InitStats()
        {
            base.InitStats();

            //if we have item data assigned
            if (itemData)
            {
                //if we have no firearm data
                if (!firearmData)
                {
                    //cast the firearm data from our item data
                    firearmData = (FirearmData)itemData;
                }
            }

            //FIREARM
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
            //if we have no item sound assigned
            if (!itemSound) Debug.LogWarning("No Item Sound found for " + gameObject.name + "; Assign Sound Reference!");

            //if we have item sound assigned
            else if (itemSound)
            {
                //if we have no firearm sound
                if (!firearmSound)
                {
                    //then cast from our item sound
                    firearmSound = (FirearmSound)itemSound;
                }
            }

            //if we have no melee sound
            if (!meleeSound)
            {
                //then grab from our object
                meleeSound = gameObject.GetComponent<MeleeSound>();
            }

            //if we have no item sway assigned
            if (!itemSway) Debug.LogWarning("No Item Sway found for " + gameObject.name + "; Assign Sway Reference!");

            //if we have item sway assigned
            else if (itemSway)
            {
                //if we have no weapon sway
                if (!weaponSway)
                {
                    //then cast from our item sway
                    weaponSway = (WeaponSway)itemSway;
                }

                //if we have no firearm sway
                if (!firearmSway)
                {
                    //then cast from our item sway
                    firearmSway = (FirearmSway)itemSway;
                }
            }

            //if we have no item animation assigned
            if (!itemAnimation) Debug.LogWarning("No Item Animation found for " + gameObject.name + "; Assign Animation Reference!");

            //if we have item animation assigned
            else if (itemAnimation)
            {
                //if we have no melee animation
                if (!meleeAnimation)
                {
                    //then cast from our item animation
                    meleeAnimation = (MeleeAnimation)itemAnimation;
                }

                //if we have no firearm animation
                if (!firearmAnimation)
                {
                    //then cast from our item animation
                    firearmAnimation = (FirearmAnimation)itemAnimation;
                }
            }
        }

        public override void ToggleEquip(bool b)
        {
            isEquipped = b;

            //if we have equipped the weapon, play the equip sound.
            if (isEquipped && firearmSound)
            {
                firearmSound.PlayEquipSound();
            }
        }

        private void Start()
        {
            //if this weapon is not equipped, then return;
            if (!isEquipped) return;
        }

        private void Update()
        {
            //if this weapon is not equipped, then return;
            if (!isEquipped) return;

            Inputs(); Animation();
        }

        protected override void Inputs()
        {
            previousMeleeInput = meleeInput;

            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            if (Input.GetKeyDown(KeyCode.R)) reloadInput = true;
            else if (Input.GetKeyUp(KeyCode.R)) reloadInput = false;

            if (Input.GetKeyDown(KeyCode.V)) meleeInput = true;
            else if (Input.GetKeyUp(KeyCode.V)) meleeInput = false;

            CheckAttack();
        }

        protected override void CheckAttack()
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

                    if (leftMouseInput) FirearmStartAttack();

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

                    if (leftMouseInput) FirearmStartAttack();

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

                        FirearmStartAttack();
                    }

                    break;
            }

            if (playerStats)
            {
                //check for stamina
                float stamina = playerStats.GetCurrentStamina();

                //if were not attacking and we press the l-mouse button or melee input
                if (meleeInput)
                {
                    //So in code, ima have to check if we have mouse input, either start a timer or invoke some methods, then if we let go of the mouse before the specified time,
                    //cancel the timer/invoke and do a light attack. However, if we go beyond the timer or we don't cancel the method invoke (because we held down the mouse),
                    //then do a heavy attack. All before this, however, we need to check if we even have any stamina,
                    //because if we don't, then we should perform an exhausted attack. Yeah, that sounds good…

                    //if we're already charged up/are charging up, just return out of this, since we can only attack when we release the mouse button
                    if (isCharged || isCharging) return;

                    //if we are invoking the charge function
                    if (IsInvoking(nameof(Charge)))
                    {
                        //if we're not charged or not charging (which should be an impossibility
                        if (!isCharged && !isCharging)
                        {
                            Debug.LogWarning("calling the Charge function, even though we are not charged or charging?! this may be a bug");
                        }

                        //else just return, since we should be charging
                        else return;
                    }

                    //else if we're not invoking the charge, and we have not already charged, and we have stamina
                    else if (!IsInvoking(nameof(Charge)) && !isCharged && !isCharging && stamina > 0)
                    {
                        StartCharge(); return;
                    }

                    else if (stamina <= 0)
                    {
                        //do an exhausted attack!
                        Debug.Log("Exhausted Attack!");
                        SetupAttack(exhaustedAttackTimeToHit, exhaustedAttackRate, exhaustedAttackDamage, 0);
                    }
                }

                //if we release the left mouse button or melee input
                else if (!meleeInput)
                {
                    //if we we're have not been holding down the mouse
                    if (previousMeleeInput == meleeInput)
                    {
                        //call the reset charge method, just in case
                        ResetCharge();
                    }

                    //if we were holding down the mouse in teh last frame
                    else if (previousMeleeInput != meleeInput)
                    {
                        //if we have charged
                        if (isCharged)
                        {
                            //do a heavy attack!
                            Debug.Log("Heavy Attack!");
                            SetupAttack(heavyAttackTimeToHit, heavyAttackRate, heavyAttackDamage, heavyAttackStaminaCost);
                        }

                        //if we have not charged
                        else if (!isCharged)
                        {
                            //if we are charging
                            if (isCharging)
                            {
                                //cancel the charge
                                CancelInvoke(nameof(Charge));
                            }

                            //if we have stamina
                            if (stamina > 0)
                            {
                                //do the light attack
                                Debug.Log("Light Attack!");
                                SetupAttack(lightAttackTimeToHit, lightAttackRate, lightAttackDamage, lightAttackStaminaCost);
                            }
                        }

                        //and call the reset charge method
                        ResetCharge();
                    }
                }
            }

            //if we dont have the player stats reference
            else if (!playerStats)
            {
                Debug.LogWarning("No Player Stats Detected! Please Assign the Player Stats!");
                return;
            }
        }

        //starts shooting
        private void FirearmStartAttack()
        {
            isAttacking = true; firearmAttack = true;

            //shoot immedietly, as this is a gun
            FirearmAttack();

            //if we have the firearm sound reference, call the play firing sound
            if (firearmSound) firearmSound.PlayFiringSound();

            //invoke end shoot after our rate of fire
            Invoke(nameof(FirearmEndAttack), 1 / firearmAttackRate);
        }

        //shoots!
        private void FirearmAttack()
        {
            //call the hit function, passing through the player camera
            FirearmHit(playerCamera);

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
        private void FirearmEndAttack()
        {
            isAttacking = false; firearmAttack = false;
        }

        private void FirearmHit(Transform point)
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

        protected override void Animation()
        {
            //if we have the animation reference
            if (firearmAnimation)
            {
                //FIREARM

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
                else if (isAiming && !isAttacking) 
                {
                    firearmAnimation.PlayFirearmAim();
                    return;
                }

                //if we're aiming the gun and we're shooting, play this anim
                else if (isAiming && isAttacking) 
                {
                    firearmAnimation.PlayFirearmAimFire();
                    return;
                }

                //if we're shooting, play this anim
                else if (isAttacking && firearmAttack) 
                {
                    firearmAnimation.PlayFirearmFire();
                    return;
                }

                //MELEE

                //if we're charging our heavy attack, then play this anim
                else if (isCharged || isCharging)
                {
                    meleeAnimation.PlayMeleeChargeAttack();
                    return;
                }

                //if we're attacking in melee
                else if (isAttacking && meleeAttack)
                {
                    //if our damage is the light attack damage, play this anim
                    if (currentDamage == lightAttackDamage)
                    {
                        meleeAnimation.PlayMeleeLightAttack();
                        return;
                    }

                    //if our damage is the heavy attack damage, play this anim
                    else if (currentDamage == heavyAttackDamage)
                    {
                        meleeAnimation.PlayMeleeHeavyAttack();
                        return;
                    }

                    //if our damage is the exhausted attack damage, play this anim
                    else if (currentDamage == exhaustedAttackDamage)
                    {
                        meleeAnimation.PlayMeleeExhaustedAttack();
                        return;
                    }
                }

                //ITEM

                //if we're not attacking, play this anim
                else if (!isAttacking)
                {
                    firearmAnimation.PlayIdle();
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

        public FirearmData GetFirearmData()
        {
            if (firearmData != null) return firearmData;

            else return null;
        }

        public void SetFirearmData(FirearmData firearmData)
        {
            this.firearmData = firearmData;

            Init();
        }
    }
}