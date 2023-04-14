using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Food : InteractableItem
    {
        [Header("Attributes")]
        [SerializeField] private int amount;

        public override bool CanInteract()
        {
            Vector2Int i = new Vector2Int(GameManager.PlayerStats.GetCurrentHunger(), GameManager.PlayerStats.GetMaxHunger());

            if (i.x < i.y) return true;
            else return false;
        }

        public override void Interact()
        {
            if (CanInteract()) Eat();
        }

        void Eat()
        {
            GameManager.PlayerStats.ReplenishHunger(amount);
            Destroy(gameObject);
        }
    }
}