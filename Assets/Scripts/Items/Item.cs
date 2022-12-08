using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public class Item : MonoBehaviour
    {
        [Header("Attributes - Item")]
        [SerializeField] protected ItemData itemData;

        [Space]
        [SerializeField] protected GameObject model;

        [Space]
        [SerializeField] protected string itemName;
        [SerializeField] protected double itemWeight;

        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (!itemData)
            {
                Debug.LogWarning("No Item Data found for " + gameObject.name + "; Assign Item Data!");
                return;
            }

            itemName = itemData.itemName;
            itemWeight = itemData.itemWeight;
        }
    }
}