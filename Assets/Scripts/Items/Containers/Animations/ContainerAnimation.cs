using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ContainerAnimation : AnimationHandler
    {
        const string IDLE = "idle";
        const string LOOTING = "looting";

        public void PlayIdle() => PlayAnimation(IDLE);
        public void PlayLooting() => PlayAnimation(LOOTING);
    }
}