using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WeaponRecoil : MonoBehaviour
    {
        [Header("Recoil Settings")]
        [SerializeField] private float recoilSpeed;
        [SerializeField] private float recoilReturnSpeed;

        [Space]
        [SerializeField] private Vector3 hipfireRecoilRotation = new Vector3(2f, 2f, 2f);
        [SerializeField] private Vector3 aimingRecoilRotation = new Vector3(0.5f, 0.5f, 1.5f);

        private bool isAiming;

        private Vector3 currentRotation, rotation;

        void Update() => UpdateRecoil();

        public void Aim(bool b) => isAiming = b;

        public void Recoil()
        {
            if (!isAiming)
            {
                currentRotation += new Vector3(-hipfireRecoilRotation.x, Random.Range(-hipfireRecoilRotation.y, hipfireRecoilRotation.y), Random.Range(-hipfireRecoilRotation.z, hipfireRecoilRotation.z));
            }

            else if (isAiming)
            {
                currentRotation += new Vector3(-aimingRecoilRotation.x, Random.Range(-aimingRecoilRotation.y, aimingRecoilRotation.y), Random.Range(-aimingRecoilRotation.z, aimingRecoilRotation.z));
            }
        }

        private void UpdateRecoil()
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
            rotation = Vector3.Slerp(rotation, currentRotation, recoilSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}