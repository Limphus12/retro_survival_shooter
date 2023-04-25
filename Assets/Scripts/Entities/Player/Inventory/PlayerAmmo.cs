using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum AmmoType { PISTOL }
    public class PlayerAmmo : MonoBehaviour
    {
        public static int PISTOL_AMMO;
        public static readonly int PISTOL_AMMO_MAX = 24;
        
        /// <summary>
        /// Returns the max ammo capacity based on the ammo type.
        /// </summary>
        /// <param name="ammo">The ammo type to specify.</param>
        /// <returns></returns>
        public static int GetMaxAmmo(AmmoType ammo)
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: return PISTOL_AMMO_MAX;
            }

            return 0;
        }

        public static int GetAmmo(AmmoType ammo)
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: return PISTOL_AMMO;
            }

            return 0;
        }

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
        public static void SetAmmo(AmmoType ammo, int amount) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO = amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, PISTOL_AMMO_MAX); break;
            }
        }

        /// <summary>
        /// Adds a certain amount to a specified ammo; clamped between 0 and a maximum amount.
        /// </summary>
        /// <param name="ammo">This is the ammo we want to increase.</param>
        /// <param name="amount">This is the amount we want to add to the ammo.</param>
        public static void AddAmmo(AmmoType ammo, int amount) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO += amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, PISTOL_AMMO_MAX); break;
            }
        }
        
        /// <summary>
        /// Removes a certain amount from a specified ammo; clamped between 0 and a maximum amount.
        /// </summary>
        /// <param name="ammo">This is the ammo we want to decrease.</param>
        /// <param name="amount">This is the amount we want to remove from the ammo.</param>
        public static void RemoveAmmo(AmmoType ammo, int amount) 
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: PISTOL_AMMO -= amount; PISTOL_AMMO = Mathf.Clamp(PISTOL_AMMO, 0, PISTOL_AMMO_MAX); break;
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

        /// <summary>
        /// 
        /// Checks if we have more than the amount of a specified ammo remaining.
        /// </summary>
        /// <param name="ammo">The ammo we want to check.</param>
        /// <param name="amount">The amount we want to check.</param>
        /// <returns></returns>
        public static bool HasAmmo(AmmoType ammo, int amount)
        {
            switch (ammo)
            {
                case AmmoType.PISTOL: if (PISTOL_AMMO >= amount) return true; else return false;
            }

            return false;
        }
    }
}