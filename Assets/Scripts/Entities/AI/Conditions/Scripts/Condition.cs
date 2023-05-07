using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Condition : ScriptableObject
    {
        // Check if the condition is true for the given AIManager
        public abstract bool CheckCondition(AIManager ai);
    }
}