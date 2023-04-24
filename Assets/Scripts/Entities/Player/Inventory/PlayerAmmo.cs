using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum AmmoType { PISTOL }
    public class PlayerAmmo : MonoBehaviour
    {
        public static int PISTOL_AMMO;
        public static readonly int PISTOL_AMMO_MAX = 18;
        
        /// <summary>
        /// Resets all ammo types back to 0.
        /// </summary>
        public static void ResetAmmo()
        {
            PISTOL_AMMO = 0;
        }

        /// <summary>
        /// Sets a certain ammo to a specified amount; clamped between 0 and a maximum amount.
        /// </summary>
        /// <param name="ammo">This is the ammo we want to set.</param>
        /// <param name="amount">This is the amount we want to set the ammo to.</param>
        /// <param name="ammoMax">This is the maximum amount that the ammo can be at.</param>
        public static void SetAmmo(AmmoType ammo, int amount, int ammoMax) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO = amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, ammoMax); break;
            }
        }

        /// <summary>
        /// Adds a certain amount to a specified ammo; clamped between 0 and a maximum amount.
        /// </summary>
        /// <param name="ammo">This is the ammo we want to increase.</param>
        /// <param name="amount">This is the amount we want to add to the ammo.</param>
        /// <param name="ammoMax">This is the maximum amount that the ammo can be at.</param>
        public static void AddAmmo(AmmoType ammo, int amount, int ammoMax) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO += amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, ammoMax); break;
            }
        }
        
        /// <summary>
        /// Removes a certain amount from a specified ammo; clamped between 0 and a maximum amount.
        /// </summary>
        /// <param name="ammo">This is the ammo we want to decrease.</param>
        /// <param name="amount">This is the amount we want to remove from the ammo.</param>
        /// <param name="ammoMax">This is the maximum amount that the ammo can be at.</param>
        public static void RemoveAmmo(AmmoType ammo, int amount, int ammoMax) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO -= amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, ammoMax); break;
            }
        }

        /// <summary>
        /// Checks if we have any of a specified ammo remaining.
        /// </summary>
        /// <param name="ammo">The ammo we want to check.</param>
        /// <returns></returns>
        public static bool HasAmmo(AmmoType ammo)
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: if (PISTOL_AMMO > 0) return true; else return false;
            }

            return false;
        }
    }
}