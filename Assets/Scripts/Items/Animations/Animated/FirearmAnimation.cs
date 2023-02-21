using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class FirearmAnimation : ItemAnimation
    {
        const string FIREARM_AIM = "firearm_aim";
        const string FIREARM_FIRE = "firearm_fire";
        const string FIREARM_AIM_FIRE = "firearm_aim_fire";
        const string FIREARM_RELOAD = "firearm_reload";
        const string FIREARM_COCK = "firearm_cock";

        public void PlayFirearmAim() => PlayAnimation(FIREARM_AIM);
        public void PlayFirearmFire() => PlayAnimation(FIREARM_FIRE);
        public void PlayFirearmAimFire() => PlayAnimation(FIREARM_AIM_FIRE);
        public void PlayFirearmReload() => PlayAnimation(FIREARM_RELOAD);
        public void PlayFirearmCock() => PlayAnimation(FIREARM_COCK);
    }
}