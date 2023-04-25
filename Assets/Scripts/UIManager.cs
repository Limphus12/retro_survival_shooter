using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI - Sliders")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider hungerSlider, thirstSlider, staminaSlider, meleeStaminaSlider, tempuratureSlider;

        [Header("UI - GameObjects")]
        [SerializeField] private GameObject lootUI;
        [SerializeField] private GameObject statsUI, inventoryUI, crosshairUI, healthUI, deathUI;

        [Header("UI - Text")]
        [SerializeField] private TextMeshProUGUI itemUINameText;
        [SerializeField] private TextMeshProUGUI itemUIAmountText, worldBorderText, interactionText;

        [Space]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI hungerText, thirstText, staminaText, meleeStaminaText;

        [Header("UI - Animation")]
        [SerializeField] private Animator worldBorderAnimator, deathAnimator; 

        private PlayerStats playerStats;
        private PlayerInventory playerInventory;
        private PlayerInteraction playerInteraction;

        public static float UIScale = 1f;
        
        // Start is called before the first frame update
        void Start()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            if (!playerInteraction) playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            //need to subscribe to events in the playerstats class.
            playerStats.OnHealthChanged += PlayerStatsOnHealthChanged;

            playerStats.OnHungerChanged += PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged += PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged += PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged += PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged += PlayerStatsOnTemperatureChanged;

            //and subscribe to the event in the world border class.
            WorldBorder.OnBorderChanged += WorldBorderOnBorderChanged;

            ScaleUI();
        }

        void OnEnable()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            if (!playerInteraction) playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            //need to subscribe to events in the playerstats class.
            playerStats.OnHealthChanged += PlayerStatsOnHealthChanged;

            playerStats.OnHungerChanged += PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged += PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged += PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged += PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged += PlayerStatsOnTemperatureChanged;

            //and subscribe to the event in the world border class.
            WorldBorder.OnBorderChanged += WorldBorderOnBorderChanged;
        }

        void OnDisable()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            if (!playerInteraction) playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            //need to unsubscribe to events in the playerstats class.
            playerStats.OnHealthChanged -= PlayerStatsOnHealthChanged;

            playerStats.OnHungerChanged -= PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged -= PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged -= PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged -= PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged -= PlayerStatsOnTemperatureChanged;

            //need to unsubscribe to the event in the world border class.
            WorldBorder.OnBorderChanged -= WorldBorderOnBorderChanged;
        }

        private void OnDestroy()
        {
            //make sure we have our playerstats
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            if (!playerInteraction) playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            //need to unsubscribe to events in the playerstats class.
            playerStats.OnHealthChanged -= PlayerStatsOnHealthChanged;

            playerStats.OnHungerChanged -= PlayerStatsOnHungerChanged;
            playerStats.OnThirstChanged -= PlayerStatsOnThirstChanged;
            playerStats.OnStaminaChanged -= PlayerStatsOnStaminaChanged;
            playerStats.OnMeleeStaminaChanged -= PlayerStatsOnMeleeStaminaChanged;
            playerStats.OnTemperatureChanged -= PlayerStatsOnTemperatureChanged;
            
            //need to unsubscribe to the event in the world border class.
            WorldBorder.OnBorderChanged -= WorldBorderOnBorderChanged;
        }

        bool uiToggle = true;

        private void Update()
        {
            CheckUI();

            CheckDeathUI();

            if (Input.GetKeyDown(KeyCode.F1))
            {
                uiToggle = !uiToggle;

                //togggle some of the ui, so we can have a cleaner screen
                statsUI.SetActive(uiToggle); inventoryUI.SetActive(uiToggle); crosshairUI.SetActive(uiToggle); healthUI.SetActive(uiToggle);
            }
        }

        bool death = false;

        private void CheckDeathUI()
        {
            if (GameManager.PlayerStats.IsDead && !death)
            {
                death = true;

                deathUI.SetActive(true);

                deathAnimator.Play("death");

                Invoke(nameof(ReturnToMainMenu), 10f);
            }
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadSceneAsync(0);
        }

        private void CheckUI()
        {
            if (playerInteraction && interactionText)
            {
                string text = "";

                if (playerInteraction.Interactable == null) text = "";
                else if (playerInteraction.CanInteract) text = "[E]";
                else if (!playerInteraction.CanInteract) text = "Cannot Interact";

                interactionText.text = text;
            }

            if (playerInventory && itemUINameText && itemUIAmountText)
            {
                GameObject item = playerInventory.GetCurrentItem();

                if (item != null)
                {
                    Item itemScript = item.GetComponent<Item>();

                    if (itemScript != null)
                    {   
                        if (itemScript.GetItemData() != null) itemUINameText.text = "" + itemScript.GetItemData().itemName;

                        Melee melee = itemScript.GetMelee();
                        Firearm firearm = itemScript.GetFirearm();
                        Throwable throwable = itemScript.GetThrowable();
                        Placeable placeable = itemScript.GetPlaceable();
                        Consumable consumable = itemScript.GetConsumable();

                        if (melee)
                        {
                            itemUIAmountText.text = "Melee";
                        }

                        else if (firearm)
                        {
                            string text = ""; //setting our text to be either the infinte stuff or the current mag & ammo counts

                            if (firearm.Magazine.InfinteClip) text += "Inf."; else text += firearm.Magazine.CurrentMagazineCount;

                            if (firearm.Magazine.InfinteAmmo) text += " / Inf."; else text += " / " + PlayerAmmo.GetAmmo(firearm.Magazine.AmmoType);

                            itemUIAmountText.text = text;

                            if (crosshairUI && uiToggle) crosshairUI.SetActive(!firearm.IsAiming);
                        }

                        else if (throwable)
                        {
                            itemUIAmountText.text = "Throwable";
                        }

                        else if (placeable)
                        {
                            itemUIAmountText.text = "Placeable";
                        }

                        else if (consumable)
                        {
                            itemUIAmountText.text = "" + consumable.GetRemainingUsageAmount();
                        }

                        else
                        {
                            itemUIAmountText.text = "Inf.";
                        }
                    }
                }
            }
        }

        private void ScaleUI()
        {
            //scale the ui elements
            statsUI.transform.localScale = Vector2.one * UIScale;
            inventoryUI.transform.localScale = Vector2.one * UIScale;
            healthUI.transform.localScale = Vector2.one * UIScale;
        }

        #region Events

        private void PlayerStatsOnHealthChanged(object sender, Events.OnIntChangedEventArgs e)
        {
            if (healthText) healthText.text = "" + e.i;
            if (healthSlider) healthSlider.value = e.i;
        }

        private void PlayerStatsOnHungerChanged(object sender, Events.OnIntChangedEventArgs e)
        {
            if (hungerText) hungerText.text = "" + e.i;
            if (hungerSlider) hungerSlider.value = e.i;
        }

        private void PlayerStatsOnThirstChanged(object sender, Events.OnIntChangedEventArgs e)
        {
            if (thirstText) thirstText.text = "" + e.i;
            if (thirstSlider) thirstSlider.value = e.i;
        }

        private void PlayerStatsOnStaminaChanged(object sender, Events.OnIntChangedEventArgs e)
        {
            if (staminaText) staminaText.text = "" + e.i;
            if (staminaSlider) staminaSlider.value = e.i;
        }

        private void PlayerStatsOnMeleeStaminaChanged(object sender, Events.OnIntChangedEventArgs e)
        {
            if (meleeStaminaText) meleeStaminaText.text = "" + e.i;
            if (meleeStaminaSlider) meleeStaminaSlider.value = e.i;
        }

        private void PlayerStatsOnTemperatureChanged(object sender, PlayerStats.OnTemperatureChangedEventArgs e)
        {
            
        }

        private void WorldBorderOnBorderChanged(object sender, Events.OnStringChangedEventArgs e)
        {
            if (worldBorderText) worldBorderText.text = e.i;
            
            if (worldBorderAnimator) 
            {
                if (e.i.Contains("Exiting")) worldBorderAnimator.SetTrigger("FadeOut");
                else if (e.i.Contains("Entering")) worldBorderAnimator.SetTrigger("FadeIn");
            }
        }

        #endregion
    }
}