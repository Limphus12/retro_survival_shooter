using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Medicine : InteractableItem
    {
        [Header("Attributes")]
        [SerializeField] private int amount;

        public override bool CanInteract()
        {
            Vector2Int i = new Vector2Int(GameManager.PlayerStats.GetCurrentHealth(), GameManager.PlayerStats.GetMaxHealth());

            if (i.x < i.y) return true;
            else return false;
        }

        public override void Interact()
        {
            if (CanInteract()) Heal();
        }

        void Heal()
        {
            GameManager.PlayerStats.ReplenishHealth(amount);
            Destroy(gameObject);
        }
    }
}