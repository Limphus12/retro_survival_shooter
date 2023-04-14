using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace com.limphus.retro_survival_shooter
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI - Text")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI hungerText, thirstText, staminaText, meleeStaminaText, temperatureText;

        [Header("UI - Sliders")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Slider meleeStaminaSlider;
        [SerializeField] private Slider tempuratureSlider;

        [Header("UI - Loot")]
        [SerializeField] private GameObject lootUI;

        [Header("UI - Item")]
        [SerializeField] private TextMeshProUGUI itemUINameText;
        [SerializeField] private TextMeshProUGUI itemUIAmountText;

        private PlayerStats playerStats;
        private PlayerInventory playerInventory;
        
        // Start is called before the first frame update
        void Start()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

            //need to subscribe to events in the playerstats class.
            playerStats.OnHealthChanged += PlayerStatsOnHealthChanged;

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
            playerStats.OnHealthChanged += PlayerStatsOnHealthChanged;

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
            playerStats.OnHealthChanged -= PlayerStatsOnHealthChanged;

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
            playerStats.OnHealthChanged -= PlayerStatsOnHealthChanged;

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
            if (playerInventory && itemUINameText && itemUIAmountText)
            {
                GameObject item = playerInventory.GetCurrentItem();

                if (item != null)
                {
                    Item itemScript = item.GetComponent<Item>();

                    if (itemScript != null)
                    {   
                        itemUINameText.text = "" + itemScript.GetItemData().itemName;

                        Melee melee = itemScript.GetMelee();

                        if (melee)
                        {
                            itemUIAmountText.text = "Melee";
                            return;
                        }

                        Firearm firearm = itemScript.GetFirearm();

                        if (firearm)
                        {
                            string text = ""; //setting our text to be either the infinte stuff or the current mag & ammo counts

                            if (firearm.Magazine.InfinteClip) text += "Inf."; else text += firearm.Magazine.CurrentMagazineCount;

                            if (firearm.Magazine.InfinteAmmo) text += " / Inf."; else text += " / " + firearm.Magazine.CurrentAmmoReserves;

                            itemUIAmountText.text = text; return;
                        }

                        Throwable throwable = itemScript.GetThrowable();

                        if (throwable)
                        {
                            itemUIAmountText.text = "Throwable";
                            return;
                        }

                        Placeable placeable = itemScript.GetPlaceable();

                        if (placeable)
                        {
                            itemUIAmountText.text = "Placeable";
                            return;
                        }

                        Consumable consumable = itemScript.GetConsumable();

                        if (consumable)
                        {
                            itemUIAmountText.text = "" + consumable.GetRemainingUsageAmount();
                            return;
                        }
                    }
                }
            }
        }

        #region Events

        private void PlayerStatsOnHealthChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (healthSlider) healthSlider.value = e.i;
            if (healthText) healthText.text = "" + e.i;
        }

        private void PlayerStatsOnHungerChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (hungerSlider) hungerSlider.value = e.i;
            if (hungerText) hungerText.text = ""+ e.i;
        }

        private void PlayerStatsOnThirstChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (thirstSlider) thirstSlider.value = e.i;
            if (thirstText) thirstText.text = "" + e.i;
        }

        private void PlayerStatsOnStaminaChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (staminaSlider) staminaSlider.value = e.i;
            if (staminaText) staminaText.text = "" + e.i;
        }

        private void PlayerStatsOnMeleeStaminaChanged(object sender, PlayerStats.OnIntChangedEventArgs e)
        {
            if (meleeStaminaSlider) meleeStaminaSlider.value = e.i;
            if (meleeStaminaText) meleeStaminaText.text = "" + e.i;
        }

        private void PlayerStatsOnTemperatureChanged(object sender, PlayerStats.OnTemperatureChangedEventArgs e)
        {
            if (temperatureText) temperatureText.text = e.i.ToString();
        }

        #endregion
    }
}