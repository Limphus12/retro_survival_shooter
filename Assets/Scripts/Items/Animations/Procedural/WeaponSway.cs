using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WeaponSway : ItemSway
    {
        [Header("Weapon - Aiming Settings")]
        [SerializeField] protected Vector3 aimingPosition;
        [SerializeField] protected Quaternion aimingRotation;

        [Space]
        [SerializeField] protected float aimingSwaySmooth = 8.0f;
        [SerializeField] protected float aimingSwayAmount = 2.0f, aimingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float aimingTiltSmooth = 8.0f;
        [SerializeField] protected float aimingTiltAmount = 2.0f, aimingTiltMaximum = 2.0f;

        protected bool isAiming;

        public void Aim(bool b) => isAiming = b;

        protected override void CheckSway()
        {
            if (isAiming) Sway(aimingSwayAmount, aimingSwayMaximum, aimingSwaySmooth, aimingPosition);

            else Sway(defaultSwayAmount, defaultSwayMaximum, defaultSwaySmooth, defaultPosition);
        }

        protected override void CheckTilt()
        {
            if (isAiming) Tilt(aimingTiltAmount, aimingTiltMaximum, aimingTiltSmooth, aimingRotation);

            else Tilt(defaultTiltAmount, defaultTiltMaximum, defaultTiltSmooth, defaultRotation);
        }
    }
}