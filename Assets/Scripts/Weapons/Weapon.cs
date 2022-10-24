using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum WeaponType { MELEE, RANGED }
    public enum WeaponFireType { SEMI, BURST, AUTO, MULTI, BOLT }

    public class Weapon : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponFireType weaponFireType;

        [SerializeField] private float rateOfFire, damage;

        [SerializeField] private string weaponName;

        [Tooltip("Used for the MULTI weaponFireType, dictates the fire points for each bullet.")]
        [SerializeField] private Transform[] multiShotTransforms;   
        
        [Tooltip("Used for the Melee weaponType, dictates the range of the punch")]
        [SerializeField] private float meleeRange;

        [SerializeField] private float boltTime;

        [Space]
        [SerializeField] private int burstAmount;
        [SerializeField] private float burstInterval;

        [Header("Weapon FX")]
        [SerializeField] private bool weaponFX;

        [Space]
        [SerializeField] private Transform firePoint;
        [SerializeField] private AudioSource weaponSound;
        [SerializeField] private GameObject muzzleGFX, muzzleLight;

        [Space]
        [SerializeField] private Transform[] multiFireGFX;

        [Header("Bullet Variables")]
        [SerializeField] private AnimationCurve bulletDropOffCurve;

        [Header("Weapon Sway")]
        [SerializeField] private WeaponSway weaponSway;

        [Header("Weapon Recoil")]
        [SerializeField] private WeaponRecoil weaponRecoil;
        [SerializeField] private WeaponRecoil cameraRecoil;


        private bool isShooting, isCocking, isBursting, burst, burstResetInvoked, isAiming;

        private int currentAmmo, currentBurstCounter;

        private Vector3 initialPosition;

        private Transform playerCameraTransform;

        // Start is called before the first frame update
        void Start()
        {
            if (!playerCameraTransform) playerCameraTransform = gameObject.GetComponentInParent<Camera>().transform;

            initialPosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update() => Inputs();

        //Detects the player input on the mouse, and checks if the weapon is currently shooting. If not, then shoot.
        void Inputs()
        {
            switch (weaponType)
            {
                case WeaponType.MELEE:

                    if (!isShooting && Input.GetMouseButtonDown(0))
                    {
                        //Punch();

                        Invoke(nameof(ResetShoot), 1 / rateOfFire);
                    }

                    break;

                case WeaponType.RANGED:

                    switch (weaponFireType)
                    {
                        case WeaponFireType.SEMI:
                            if (!isShooting && Input.GetMouseButtonDown(0))
                            {
                                Shoot();

                                Invoke(nameof(ResetShoot), 1 / rateOfFire);
                            }

                            break;

                        case WeaponFireType.AUTO:
                            if (!isShooting && Input.GetMouseButton(0))
                            {
                                Shoot();

                                Invoke(nameof(ResetShoot), 1 / rateOfFire);
                            }

                            break;

                        case WeaponFireType.MULTI:
                            if (!isShooting && Input.GetMouseButtonDown(0))
                            {
                                MultiShoot();

                                Invoke(nameof(ResetShoot), 1 / rateOfFire);
                            }

                            break;

                        case WeaponFireType.BURST:
                            if (!isShooting && Input.GetMouseButtonDown(0) && !isBursting && !burst)
                            {
                                StartBurst();
                            }

                            break;

                        case WeaponFireType.BOLT:
                            if (!isShooting && Input.GetMouseButtonDown(0) && !isCocking)
                            {
                                Shoot();

                                StartCock();
                            }

                            break;

                        default:
                            break;
                    }

                    break;
            }

            if (isBursting) CheckBurst();
            
            if (Input.GetMouseButtonDown(1)) Aim();
        }

        private void Aim()
        {
            isAiming = !isAiming;

            if (weaponSway && weaponRecoil && cameraRecoil)
            {
                weaponSway.Aim(isAiming);
                weaponRecoil.Aim(isAiming);
                cameraRecoil.Aim(isAiming);
            }
        }

        //Fires a single raycast from the camera, out forward.
        void Shoot()
        {
            isShooting = true;

            //do raycasts to hit enemies
            ShootRaycast(playerCameraTransform);

            //do recoil
            if (weaponRecoil && cameraRecoil)
            {
                weaponRecoil.Recoil();
                cameraRecoil.Recoil();
            }

            //Weapon effects, such as muzzle flash, weapon firing sounds etc.
            if (weaponFX)
            {
                MuzzleGFX();
                StartCoroutine(MuzzleFlash()); 
                weaponSound.Play();
            }
        }

        //Similar to the Shoot Method above, but loops over an array of transforms and fires raycasts from each.
        void MultiShoot()
        {
            isShooting = true;

            if (weaponFX)
            {
                MuzzleGFX();
                StartCoroutine(MuzzleFlash()); 
                weaponSound.Play(); 
            }

            for (int i = 0; i < multiShotTransforms.Length; i++)
            {
                ShootRaycast(multiShotTransforms[i]);
            }
        }

        void ShootRaycast(Transform point)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, Mathf.Infinity))
            {
                Health hp = hit.transform.GetComponent<Health>();

                if (hp) hp.TakeDamage(damage);
            }
        }

        //just resets isShooting to false :D
        void ResetShoot() => isShooting = false;

        void StartCock()
        {
            isCocking = true;

            Debug.Log("Cocking the weapon");

            Invoke(nameof(ResetCock), boltTime);
            Invoke(nameof(ResetShoot), 1 / rateOfFire);
        }

        void ResetCock() => isCocking = false;

        void StartBurst()
        {
            isBursting = true;

            Invoke(nameof(ResetShoot), 1 / rateOfFire);
        }

        void CheckBurst()
        {
            if (currentBurstCounter < burstAmount && !burst)
            {
                burst = true;

                Invoke(nameof(ResetBurstShot), burstInterval);
                Shoot();
            }

            else if (currentBurstCounter >= burstAmount && !burst && !burstResetInvoked)
            {
                burstResetInvoked = true;
                Invoke(nameof(ResetBurst), burstInterval);
            }
        }

        void ResetBurstShot()
        {
            currentBurstCounter++;
            burst = false;
        }

        void ResetBurst()
        {
            currentBurstCounter = 0;

            isBursting = false;
            burstResetInvoked = false;
        }

        void MuzzleGFX()
        {
            if (muzzleGFX)
            {
                GameObject mGFX = Instantiate(muzzleGFX, firePoint.position, firePoint.rotation);

                //mGFX.transform.parent = firePoint;

                Destroy(mGFX, 1f);
            }
        }

        IEnumerator MuzzleFlash()
        {
            if (muzzleLight)
            {
                muzzleLight.SetActive(true);

                yield return new WaitForSeconds(0.2f);

                muzzleLight.SetActive(false);
            }
        }
    }
}