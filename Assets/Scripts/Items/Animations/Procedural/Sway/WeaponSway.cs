using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WeaponSway : ItemSway
    {
        [Header("Attributes - Weapon Aiming Settings")]
        [SerializeField] protected Vector3 aimingPosition;
        [SerializeField] protected Quaternion aimingRotation;

        [Space]
        [SerializeField] protected float aimingSwaySmooth = 8.0f;
        [SerializeField] protected float aimingSwayAmount = 2.0f, aimingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float aimingTiltSmooth = 8.0f;
        [SerializeField] protected float aimingTiltAmount = 2.0f, aimingTiltMaximum = 2.0f;

        [Space]
        [SerializeField] protected Melee melee;

        protected bool isAiming;

        public void Aim(bool b) => isAiming = b;

        protected override void CheckSway()
        {
            if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
            {
                //if we're currently attacking or charging
                if (melee && melee.GetMeleeState() != MeleeState.IDLE)
                {
                    //if we're blocking
                    if (melee.GetMeleeState() == MeleeState.BLOCKING)
                    {
                        Sway(aimingSwayAmount, aimingSwayMaximum, aimingSwaySmooth, aimingPosition);
                    }

                    else Sway(defaultSwayAmount, defaultSwayMaximum, defaultSwaySmooth, defaultPosition);
                }

                Sway(runningSwayAmount, runningSwayMaximum, runningSwaySmooth, runningPosition);
            }

            else if (isAiming) Sway(aimingSwayAmount, aimingSwayMaximum, aimingSwaySmooth, aimingPosition);

            else Sway(defaultSwayAmount, defaultSwayMaximum, defaultSwaySmooth, defaultPosition);
        }

        protected override void CheckTilt()
        {
            if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
            {
                //if we're currently attacking or charging
                if (melee && melee.GetMeleeState() != MeleeState.IDLE)
                {
                    //if we're blocking
                    if (melee.GetMeleeState() == MeleeState.BLOCKING)
                    {
                        Tilt(aimingTiltAmount, aimingTiltMaximum, aimingTiltSmooth, aimingRotation);
                    }

                    else Tilt(defaultTiltAmount, defaultTiltMaximum, defaultTiltSmooth, defaultRotation);
                }

                Tilt(runningTiltAmount, runningTiltMaximum, runningTiltSmooth, runningRotation);
            }

            else if (isAiming) Tilt(aimingTiltAmount, aimingTiltMaximum, aimingTiltSmooth, aimingRotation);

            else Tilt(defaultTiltAmount, defaultTiltMaximum, defaultTiltSmooth, defaultRotation);
        }
    }
}