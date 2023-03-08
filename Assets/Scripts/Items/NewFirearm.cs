using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class NewFirearm : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private float damage;
        [SerializeField] private float attackRate;

        [Space]
        [SerializeField] private Transform playerCamera;

        [Header("Effects")]
        [SerializeField] private FirearmAnimation firearmAnimation;
        [SerializeField] private FirearmSound firearmSound;

        [Space]
        [SerializeField] private WeaponRecoil cameraRecoil;
        [SerializeField] private WeaponRecoil weaponRecoil;

        [Space]
        [SerializeField] private Transform firePoint;
        
        [Space]
        [SerializeField] private GameObject bulletParticles;
        [SerializeField] private GameObject muzzleParticles;

        private bool isAttacking, isAiming;

        private void Start()
        {
            if (!playerCamera) playerCamera = Camera.main.transform;

            if (!cameraRecoil) cameraRecoil = playerCamera.GetComponentInParent<WeaponRecoil>();

            if (!weaponRecoil) weaponRecoil = gameObject.GetComponentInChildren<WeaponRecoil>();

            if (!firearmAnimation) firearmAnimation = gameObject.GetComponent<FirearmAnimation>();

            if (!firearmSound) firearmSound = gameObject.GetComponent<FirearmSound>();
        }

        private void Update()
        {
            CheckInputs();
        }

        private void CheckInputs()
        {
            if (isAttacking) return;

            Aim(Input.GetMouseButton(1));

            if (firearmAnimation)
            {
                if (isAiming) firearmAnimation.PlayFirearmAim();

                else firearmAnimation.PlayIdle();
            }

            if (Input.GetMouseButton(0))
            {
                StartAttack();
            }
        }

        //starts shooting
        private void StartAttack()
        {
            isAttacking = true;

            Debug.Log("Started Attack");

            //shoot immedietly, as this is a gun
            Attack();

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndAttack), 1 / attackRate);
        }

        //shoots!
        private void Attack()
        {
            Debug.Log("Attacking!");

            //effect stuff
            if (firearmSound) firearmSound.PlayFiringSound();
            if (firearmAnimation)
            {
                if (isAiming) firearmAnimation.PlayFirearmAimFire();

                else firearmAnimation.PlayFirearmFire();
            }
            
            Effects();

            //call the hit function, passing through the player camera
            Hit(playerCamera);
        }

        //ends shooting
        private void EndAttack()
        {
            Debug.Log("Ended Attack");

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

                if (damageable != null) damageable.Damage(damage);
            }
        }

        private void Effects()
        {
            if (bulletParticles)
            {
                GameObject bullet = Instantiate(bulletParticles, firePoint.position, firePoint.rotation);
                Destroy(bullet, 10f);
            }

            if (muzzleParticles)
            {
                GameObject muzzle = Instantiate(muzzleParticles, firePoint.position, firePoint.rotation);
                Destroy(muzzle, 1f);
            }

            if (cameraRecoil) cameraRecoil.Recoil();
            if (weaponRecoil) weaponRecoil.Recoil();
        }

        private void Aim(bool b)
        {
            isAiming = b;

            if (cameraRecoil) cameraRecoil.Aim(b);
            if (weaponRecoil) weaponRecoil.Aim(b);
        }
    }
}