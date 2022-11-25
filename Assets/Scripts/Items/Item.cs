using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Item : MonoBehaviour
    {
        [Header("Attributes - Item")]
        [SerializeField] protected string itemName;
        [SerializeField] protected double weight;
    }
}