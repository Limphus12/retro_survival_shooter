using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    public class Ammo : InteractableItem
    {
        [Header("Attributes")]
        [SerializeField] private AmmoType ammoType;
        [SerializeField] private int amount;

        public override bool CanInteract()
        {
            Vector2Int i = new Vector2Int(0,0);

            switch (ammoType)
            {
                case AmmoType.PISTOL: i = new Vector2Int(PlayerAmmo.PISTOL_AMMO, PlayerAmmo.PISTOL_AMMO_MAX); break;
            }

            if (i.x < i.y) return true;
            else return false;
        }

        public override void Interact()
        {
            if (CanInteract()) ReplenishAmmo();
        }

        void ReplenishAmmo()
        {
            switch (ammoType)
            {
                case AmmoType.PISTOL: PlayerAmmo.AddAmmo(ammoType, amount); break;
            }

            Destroy(gameObject);
        }
    }
}