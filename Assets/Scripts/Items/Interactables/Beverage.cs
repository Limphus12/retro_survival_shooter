using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Beverage : InteractableItem
    {
        [Header("Attributes")]
        [SerializeField] private int amount;

        public override bool CanInteract()
        {
            Vector2Int i = new Vector2Int(GameManager.PlayerStats.GetCurrentThirst(), GameManager.PlayerStats.GetMaxThirst());

            if (i.x < i.y) return true;
            else return false;
        }

        public override void Interact()
        {
            if (CanInteract()) Drink();
        }

        void Drink()
        {
            GameManager.PlayerStats.ReplenishThirst(amount);
            Destroy(gameObject);
        }
    }
}