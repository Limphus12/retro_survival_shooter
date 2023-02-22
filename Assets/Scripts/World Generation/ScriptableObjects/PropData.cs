using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "PropData", menuName = "World Generation/Structures/Props")]
    public class PropData : ScriptableObject
    {
        [Header("Assets")]
        public GameObject[] assets;
    }
}