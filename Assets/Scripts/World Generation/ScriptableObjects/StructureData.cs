using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "StructureData", menuName = "World Generation/Structures")]
    public class StructureData : ScriptableObject
    {
        [Header("Assets")]
        public GameObject[] assets;
    }
}