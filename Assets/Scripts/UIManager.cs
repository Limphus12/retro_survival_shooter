using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace com.limphus.retro_survival_shooter
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI hungerText;
        [SerializeField] private TextMeshProUGUI thirstText, staminaText, meleeStaminaText, temperatureText;

        [Space]
        [SerializeField] private GameObject worldBorderUI, travellingUI; 
        
        private PlayerStats playerStats;
        
        // Start is called before the first frame update
        void Start()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            //need to subscribe to events in the playerstats class.
            playerStats.OnHungerChanged += PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged += PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged += PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged += PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged += PlayerStatsOnTemperatureChanged;
        }

        void OnEnable()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            //need to subscribe to events in the playerstats class.
            playerStats.OnHungerChanged += PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged += PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged += PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged += PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged += PlayerStatsOnTemperatureChanged;
        }

        void OnDisable()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            //need to unsubscribe to events in the playerstats class.
            playerStats.OnHungerChanged -= PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged -= PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged -= PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged -= PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged -= PlayerStatsOnTemperatureChanged;
        }

        private void OnDestroy()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            //need to unsubscribe to events in the playerstats class.
            playerStats.OnHungerChanged -= PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged -= PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged -= PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged -= PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged -= PlayerStatsOnTemperatureChanged;
        }

        private void Update()
        {
            CheckUI();
        }

        private void CheckUI()
        {
            if (worldBorderUI)
            {
                if (!WorldBorder.IsTravelling) worldBorderUI.SetActive(WorldBorder.IsInBorder);

                else worldBorderUI.SetActive(false);
            }

            if (travellingUI) travellingUI.SetActive(WorldBorder.IsTravelling);
        }

        #region Events

        private void PlayerStatsOnHungerChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (hungerText) hungerText.text = "Hunger - " + e.i + "/100";
        }

        private void PlayerStatsOnThirstChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (thirstText) thirstText.text = "Thirst - " + e.i + "/100";
        }

        private void PlayerStatsOnStaminaChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (staminaText) staminaText.text = "Stamina - " + e.i + "/100";
        }

        private void PlayerStatsOnMeleeStaminaChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (meleeStaminaText) meleeStaminaText.text = "Melee Stamina - " + e.i + "/100";
        }

        private void PlayerStatsOnTemperatureChanged(object sender, PlayerStats.OnTemperatureChangedEventArgs e)
        {
            if (temperatureText) temperatureText.text = "Temperature - " + e.i.ToString();
        }

        #endregion
    }
}