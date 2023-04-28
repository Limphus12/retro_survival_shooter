using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(AIManager ai);
    }
}