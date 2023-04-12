using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ContainerAnimation : AnimationHandler
    {
        const string IDLE = "idle";
        const string LOOTED = "looted";

        public void PlayIdle() => PlayAnimation(IDLE);
        public void PlayLooted() => PlayAnimation(LOOTED);
    }
}