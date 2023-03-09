using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableItem : MonoBehaviour
    {
        [SerializeField] private int hungerAmount, thirstAmount, staminaAmount, meleeStaminaAmount;

        private void OnTriggerEnter(Collider other)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats)
            {
                //add to our stats!
                playerStats.SetCurrentHunger(playerStats.GetCurrentHunger() + hungerAmount);
                playerStats.SetCurrentThirst(playerStats.GetCurrentThirst() + thirstAmount);
                playerStats.SetCurrentStamina(playerStats.GetCurrentStamina() + staminaAmount);
                playerStats.SetCurrentMeleeStamina(playerStats.GetCurrentMeleeStamina() + meleeStaminaAmount);
            }

            Destroy(gameObject);
        }
    }
}