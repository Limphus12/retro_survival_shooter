using System;
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

        [Space, SerializeField] private bool playerItem;

        public Magazine Magazine { get; private set; }

        #region Private Variables

        private int firearmDamage, magazineSize;
        private float reloadTime, firearmAttackRate;
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

        public Transform FirePoint { get; set; }

        public bool IsAttacking { get; private set; }
        public bool IsAiming { get; private set; }
        public bool IsReloading { get; private set; }
        public bool IsCocked { get; private set; }
        public bool IsCocking { get; private set; }

        #endregion

        #region Initialization

        private void Awake() => Init();

        private void Init()
        {
            if (!Magazine) Magazine = GetComponent<Magazine>();
            if (!FirePoint && playerItem) FirePoint = Camera.main.transform;

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

            if (!cameraRecoil && FirePoint) cameraRecoil = FirePoint.GetComponentInParent<WeaponRecoil>();

            if (!firearmSway) firearmSway = gameObject.GetComponent<FirearmSway>();

            if (!firearmAnimation) firearmAnimation = gameObject.GetComponent<FirearmAnimation>();

            if (!firearmSound) firearmSound = gameObject.GetComponent<FirearmSound>();

            if (!firearmFX) firearmFX = gameObject.GetComponent<FirearmFX>();

            if (!firearmFunctionAnimation) firearmFunctionAnimation = gameObject.GetComponent<FirearmFunctionAnimation>();
        }

        #endregion

        public bool InUse()
        {
            if (IsAttacking || IsReloading || IsCocking) return true;

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

                else if (!Magazine.IsReloading && !IsAttacking && reloadInput)
                {
                    Magazine.CheckReload();

                    if (Magazine.IsReloading)
                    {
                        if (firearmSound) firearmSound.PlayReloadingSound();

                        Aim(false); return;
                    }
                }
            }

            if (IsAttacking) return;

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

                    if (IsCocking) return;

                    else if (!IsCocked) { StartCock(); return; }

                    if(Magazine.CurrentMagazineCount <= 0)
                    {
                        if (Input.GetMouseButtonDown(0) && firearmSound)
                        {
                            firearmSound.PlayDryFiringSound();
                        }

                        return;
                    }

                    if (leftMouseInput && IsCocked) { IsCocked = false; StartAttack(); }

                    break;
            }
        }

        #region Attack Functions

        private void StartAttack()
        {
            IsAttacking = true;

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
            Hit(FirePoint);

            //remove ammo from the mag
            Magazine.DepleteAmmoFromMagazine(1);

            //if we have the camera and weapon recoil references, call the recoil method on them too
            if (cameraRecoil && weaponRecoil) { cameraRecoil.Recoil(); weaponRecoil.Recoil(); }
        }

        private void EndAttack() => IsAttacking = false;

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

            else IsAiming = b;

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
            if (IsCocking) return FirearmState.COCKING;
            else if (Magazine.IsReloading) return FirearmState.RELOADING;
            else if (IsAiming && !IsAttacking) return FirearmState.AIMING;
            if (IsAttacking && IsAiming) return FirearmState.AIMATTACK;
            else if (IsAttacking) return FirearmState.ATTACKING;
            else return FirearmState.IDLE;
        }

        private void StartCock()
        {
            IsCocking = true;

            if (firearmSway) firearmSway.Cock(IsCocking);
            if (firearmSound) firearmSound.PlayCockingSound();
            if (firearmFunctionAnimation) firearmFunctionAnimation.PlayFirearmCock();

            Invoke(nameof(Cock), cockTime);
        }

        private void Cock() { IsCocked = true; EndCock(); }
        private void EndCock() { IsCocking = false; if (firearmSway) firearmSway.Cock(IsCocking); }
        private void InterruptCock() { CancelInvoke(nameof(Cock)); EndCock(); }

        //a method to interrupt the firearm functions
        public void Interrupt()
        {
            if (!InUse()) return;

            else
            {
                if (Magazine && Magazine.IsReloading) Magazine.InterruptReload();

                if (IsCocking) InterruptCock();
            }
        }

        #endregion
    }
}