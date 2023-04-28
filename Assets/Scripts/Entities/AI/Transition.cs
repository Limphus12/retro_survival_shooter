using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public class Transition
    {
        public Decision decision;
        public State trueState;
        public State falseState;
    }
}