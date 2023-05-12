using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AIAttack : AIAction
    {
        [SerializeField] private Firearm firearm;

        public override void Act(AIManager ai)
        {
            CheckAttack(ai);

            ai.Walk();
        }

        private void CheckAttack(AIManager ai)
        {
            //ideally we'd have an inventory to check our weapons
            //then we'd be able to equip different items and whatnot...

            Debug.Log("Attacking!");

            ai.SetTargetPos(transform.position);

            if (firearm && !firearm.InUse())
            {
                firearm.CheckInputs(true, false, false);
            }
        }

        public bool IsAttacking => firearm.IsAttacking;
    }
}