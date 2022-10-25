using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Firearm : Weapon
    {
        [Header("Attributes - Firearm")]
        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        [Space]
        [SerializeField] private WeaponSway weaponSway;

        private bool isAiming;

        private void Update() => Inputs();

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            //leftMouseInput = Input.GetMouseButton(0);
            //rightMouseInput = Input.GetMouseButton(1);

            CheckShoot();
        }

        protected override void CheckShoot()
        {
            //check for r-mouse input and update aiming
            Aim(rightMouseInput);

            //if were shooting already, dont do anything
            if (isShooting) return;

            //if were not shooting and we press the l-mouse button, start shooting
            if (leftMouseInput) StartShoot();
        }

        //starts shooting
        protected override void StartShoot()
        {
            isShooting = true;

            //TODO - we need to deplete ammo from our current mag size.
            //were also gonna need to do some reload functions and whatnot lmao.

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

            //if we have the camera and weapon recoil references, call the aim method on them too
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
    }
}