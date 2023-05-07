using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AIAttack : AIAction
    {
        public override void Act(AIManager ai)
        {
            CheckAttack(ai);
        }

        private void CheckAttack(AIManager ai)
        {
            //ideally we'd have an inventory to check our weapons
            //then we'd be able to equip different items and whatnot...

            //oh what if we have the ai 
        }
    }
}