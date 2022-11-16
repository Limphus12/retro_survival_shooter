using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum FirearmSize { SMALL, LARGE } //prolly gonna be used to determine if we can put the gun in a holster or have to lug it on our back.
    public enum FirearmFireType { SEMI, BURST, AUTO, MULTI, BOLT } //mostly gonna be using the bolt and multi fire types, as we're using older weapons in this game.

    public class Firearm : Weapon
    {
        [Header("Attributes - Firearm")]
        [SerializeField] private int magazineSize;
        [SerializeField] private double reloadTime;

        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        [Space]
        [SerializeField] private WeaponSway weaponSway;

        [Space]
        [SerializeField] private FirearmFireType fireType;
        [SerializeField] private FirearmSize size;

        private bool isAiming, isReloading;
        private bool reloadInput;
        private int currentAmmo;

        private void Update() => Inputs();

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            if (Input.GetKeyDown(KeyCode.R)) reloadInput = true;
            else if (Input.GetKeyUp(KeyCode.R)) reloadInput = false;

            //leftMouseInput = Input.GetMouseButton(0);
            //rightMouseInput = Input.GetMouseButton(1);

            CheckShoot();
        }

        protected override void CheckShoot()
        {
            //if we press the r key and can reload, and we're not already reloading, and we can reload, then reload!
            if (reloadInput && !isReloading && (CheckReload() == 0 || CheckReload() == 1))
            {
                //make sure to stop aiming lmao.
                StartReload(); Aim(false);  return;
            }

            //if we're already reloading, dont do anything else here.
            else if (isReloading) return;

            //check for r-mouse input and update aiming
            Aim(rightMouseInput);

            //if were shooting already, dont do anything
            if (isShooting) return;

            //if were not shooting and we press the l-mouse button, start shooting
            if (leftMouseInput)
            {
                //if we have no ammo tho, cry about it
                if (CheckReload() == 0) Debug.Log("No ammo in the mag, we can't fire!");

                else StartShoot();
            }
        }

        //starts shooting
        protected override void StartShoot()
        {
            isShooting = true;

            //shoot immedietly, as this is a gun
            Shoot();

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndShoot), 1 / (float)rateOfFire);
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
            if (cameraRecoil && weaponRecoil && weaponSway)
            {
                cameraRecoil.Aim(b);
                weaponRecoil.Aim(b);
                weaponSway.Aim(b);
            }
        }

        private int CheckReload()
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("Cannot Shoot, we have no ammo!");

                return 0; //if we have no ammo in the mag
            }

            else if (currentAmmo < magazineSize)
            {
                Debug.Log("We have ammo, but not a full mag!");

                return 1; //if we dont have the full amount
            }

            else if (currentAmmo == magazineSize)
            {
                Debug.Log("We got a full mag, let 'em loose!");

                return 2; //if we are full
            }

            else return 3;
        }

        private void StartReload()
        {
            Debug.Log("Reloading!");

            //set isreloading to true and invoke our reload method
            isReloading = true;

            //if we have the weapon sway reference, call the reload method on it too
            if (weaponSway) weaponSway.Reload(isReloading);

            Invoke(nameof(Reload), (float)reloadTime);
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
            if (weaponSway) weaponSway.Reload(isReloading);
        }
    }
}