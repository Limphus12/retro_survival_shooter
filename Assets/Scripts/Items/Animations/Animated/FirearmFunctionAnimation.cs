using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class FirearmFunctionAnimation : AnimationHandler
    {
        const string FIREARM_COCK = "firearm_cock";
        const string FIREARM_UNCOCK = "firearm_uncock";

        public void PlayFirearmCock() => PlayAnimation(FIREARM_COCK);
        public void PlayFirearmUnCock() => PlayAnimation(FIREARM_UNCOCK);
    }
}