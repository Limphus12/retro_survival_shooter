using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class ItemAnimation : AnimationHandler
    {
        const string IDLE = "idle";
        const string RUNNING = "running";
        const string EQUIP = "equip";
        const string DEEQUIP = "deequip";

        public void PlayIdle() => PlayAnimation(IDLE);
        public void PlayRunning() => PlayAnimation(RUNNING);
        public void PlayEquip() => PlayAnimation(EQUIP);
        public void PlayDeEquip() => PlayAnimation(DEEQUIP);
    }
}