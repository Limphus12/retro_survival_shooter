using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class FirearmSway : WeaponSway
    {
        [Header("Firearm - Reloading Settings")]
        [SerializeField] protected Vector3 reloadingPosition;
        [SerializeField] protected Quaternion reloadingRotation;

        [Space]
        [SerializeField] protected float reloadingSwaySmooth = 8.0f;
        [SerializeField] protected float reloadingSwayAmount = 2.0f, reloadingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float reloadingTiltSmooth = 8.0f;
        [SerializeField] protected float reloadingTiltAmount = 2.0f, reloadingTiltMaximum = 2.0f;

        [Header("Firearm - Cocking Settings")]
        [SerializeField] protected Vector3 cockingPosition;
        [SerializeField] protected Quaternion cockingRotation;

        [Space]
        [SerializeField] protected float cockingSwaySmooth = 4.0f;
        [SerializeField] protected float cockingSwayAmount = 0.0f, cockingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float cockingTiltSmooth = 8.0f;
        [SerializeField] protected float cockingTiltAmount = 4.0f, cockingTiltMaximum = 8.0f;

        protected bool isReloading, isCocking;

        public void Reload(bool b) => isReloading = b;

        public void Cock(bool b) => isCocking = b;

        protected override void CheckSway()
        {
            if (isReloading) Sway(reloadingSwayAmount, reloadingSwayMaximum, reloadingSwaySmooth, reloadingPosition);

            else if (isCocking) Sway(cockingSwayAmount, cockingSwayMaximum, cockingSwaySmooth, cockingPosition);

            else if (!isAiming && !isCocking) Sway(defaultSwayAmount, defaultSwayMaximum, defaultSwaySmooth, defaultPosition);

            else if (isAiming) Sway(aimingSwayAmount, aimingSwayMaximum, aimingSwaySmooth, aimingPosition);
        }

        protected override void CheckTilt()
        {
            if (isReloading) Tilt(reloadingTiltAmount, reloadingTiltMaximum, reloadingTiltSmooth, reloadingRotation);

            else if (isCocking) Tilt(cockingTiltAmount, cockingTiltMaximum, cockingTiltSmooth, cockingRotation);

            else if (!isAiming && !isCocking) Tilt(defaultTiltAmount, defaultTiltMaximum, defaultTiltSmooth, defaultRotation);

            else if (isAiming) Tilt(aimingTiltAmount, aimingTiltMaximum, aimingTiltSmooth, aimingRotation);
        }
    }
}