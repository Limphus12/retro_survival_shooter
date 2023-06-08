using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public enum Temperature { VERY_COLD, COLD, NORMAL, HOT, VERY_HOT }

    public class PlayerStats : EntityStats
    {
        [Header("Variables - Player Survival - Initial Stats")]
        [SerializeField] private int maxHunger;
        [SerializeField] private int maxThirst, maxStamina, maxMeleeStamina;

        [Space]
        [SerializeField] private Temperature startingTemperature;

        //[Header("Variables - Player Survival - Current Stats")]
        private int currentHunger;
        private int currentThirst, currentStamina, currentMeleeStamina;
        private Temperature currentTemperature;

        [Header("Variables - Player Survival - Hunger & Thirst Depletion")]
        [Tooltip("[IN SECONDS] - How quickly Hunger depletes")] [SerializeField] private float hungerTickRate;
        [Tooltip("[IN SECONDS] - How quickly Thirst depletes")] [SerializeField] private float thirstTickRate;

        [Space]
        [Tooltip("How much Hunger is depleted per tick")] [SerializeField] private int hungerDepletionRate;
        [Tooltip("How much Thirst is depleted per tick")] [SerializeField] private int thirstDepletionRate;



        [Header("Variables - Player Survival - Hunger & Thirst Damage")]
        [Tooltip("[IN SECONDS] - How quickly damage occurs when at 0 Hunger")] [SerializeField] private float hungerDepletedTickRate;
        [Tooltip("[IN SECONDS] - How quickly damage occurs when at 0 Thirst")] [SerializeField] private float thirstDepletedTickRate;

        [Space]
        [Tooltip("How much Damage is depleted per tick when at 0 Hunger")] [SerializeField] private int hungerDepletedDamage;
        [Tooltip("How much Damage is depleted per tick when at 0 Thirst")] [SerializeField] private int thirstDepletedDamage;



        [Header("Variables - Player Survival - Stamina")]
        [Tooltip("[WHEN STAMINA IS BEING USED] How much Stamina is depleted per tick")] [SerializeField] private int staminaDepletionRate;
        [Tooltip("[WHEN STAMINA IS NOT BEING USED] How much Stamina is replenished")] [SerializeField] private int staminaReplenishRate;

        [Space]
        [Tooltip("[IN SECONDS] - How quickly Stamina replenishes")] [SerializeField] private float staminaReplenishTickRate;
        [Tooltip("[IN SECONDS] - How quickly Stamina depletes")] [SerializeField] private float staminaDepletionTickRate;

        [Space]
        [Tooltip("[IN SECONDS] The time it takes for Stamina Regen to kick in")] [SerializeField] private float staminaReplenishTime;

        [Header("Variables - Player Survival - Melee Stamina")]
        [Tooltip("[WHEN MELEE IS NOT BEING USED] How much Stamina is replenished")] [SerializeField] private int meleeStaminaReplenishRate;

        [Space]
        [Tooltip("[IN SECONDS] - How quickly Stamina replenishes")] [SerializeField] private float meleeStaminaReplenishTickRate;

        [Space]
        [Tooltip("[IN SECONDS] The time it takes for Stamina Regen to kick in")] [SerializeField] private float meleeStaminaReplenishTime;

        public event EventHandler<Events.OnIntChangedEventArgs> OnHungerChanged, OnThirstChanged, OnStaminaChanged, OnMeleeStaminaChanged;
        public event EventHandler<OnTemperatureChangedEventArgs> OnTemperatureChanged;

        #region Initialization

        private void Start()
        {
            //firstly cancel all invokes
            CancelInvoke();

            //then invoke our repeating hunger and thirst ticks
            InvokeRepeating(nameof(HungerTick), 0f, hungerTickRate);
            InvokeRepeating(nameof(ThirstTick), 0f, thirstTickRate);
        }

        protected override void InitVariables()
        {
            base.InitVariables();

            //set our current variables
            SetCurrentHunger(maxHunger);
            SetCurrentThirst(maxThirst);
            SetCurrentStamina(maxStamina);
            SetCurrentMeleeStamina(maxMeleeStamina);
            SetCurrentTemperature(startingTemperature);
        }

        #endregion

        #region Stats

        bool death = false;

        protected override void Kill()
        {
            if (!death)
            {
                death = true;

                base.Kill();

                //make it so we cannot move
                PlayerController.canMove = false;
                PlayerController.canRotate = false;
                PlayerController.canCameraLean = false;

                PlayerController pc = GameManager.PlayerController;
                if (pc) pc.CharacterController.enabled = false;

                //grab the player camera game object
                GameObject playerCamera = GameManager.PlayerCamera.gameObject;

                if (playerCamera)
                {
                    //add a rigidbody and sphere collider
                    playerCamera.transform.parent = null;

                    Rigidbody rb = playerCamera.AddComponent<Rigidbody>();
                    BoxCollider bx = playerCamera.AddComponent<BoxCollider>();

                    if (bx) bx.size = Vector3.one / 4;

                    for (int i = 0; i < playerCamera.transform.childCount; i++)
                    {
                        playerCamera.transform.GetChild(i).gameObject.SetActive(false);
                    }

                    float offsetF = 0.2f;

                    Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetF, offsetF), 0.01f, UnityEngine.Random.Range(-offsetF, offsetF));

                    if (rb) rb.AddExplosionForce(350f, playerCamera.transform.position + offset, 2f);
                }
            }
        }


        #region Hunger

        //used to grab our current hunger
        //(maybe replace with events instead?)
        public int GetCurrentHunger() => currentHunger;
        public int GetMaxHunger() => maxHunger;

        //sets our current hunger
        public void SetCurrentHunger(int amount)
        {
            currentHunger = amount;

            //doing our clamping in here
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

            //firing off our event here
            OnHungerChanged?.Invoke(this, new Events.OnIntChangedEventArgs { i = currentHunger });
        }

        //a method to deplete hunger
        public void DepleteHunger(int amount)
        {
            //decreases current hunger
            SetCurrentHunger(GetCurrentHunger() - amount);

            //checking if our hunger is 0
            if (GetCurrentHunger() <= 0)
            {
                //we have to invoke a differnt method (hungerdepletedtick) that then calls the depleteHealth method
                //cos invokes cannot pass through paramaters
                if (!IsInvoking(nameof(HungerDepletedTick)))
                {
                    InvokeRepeating(nameof(HungerDepletedTick), 0f, hungerDepletedTickRate);
                }
            }
        }

        //a method to replenish hunger
        public void ReplenishHunger(int amount)
        {
            //increaes current hunger
            SetCurrentHunger(GetCurrentHunger() + amount);

            //if we're above 0 hunger
            if (GetCurrentHunger() > 0)
            {
                //cancel the hunger depleted tick
                CancelInvoke(nameof(HungerDepletedTick));
            }

            if (GetCurrentHunger() >= maxHunger) //if we have full hunger
            {
                //then debug log that we have full hunger
                //Debug.Log("Character (" + gameObject.name + ") is at Full Hunger");
            }
        }

        #endregion

        #region Thirst

        //used to grab our current thirst
        //(maybe replace with events instead?)
        public int GetCurrentThirst() => currentThirst;
        public int GetMaxThirst() => maxThirst;
        
        //sets our current thirst
        public void SetCurrentThirst(int amount)
        {
            currentThirst = amount;

            //doing our clamping in here
            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);

            //firing off our event here
            OnThirstChanged?.Invoke(this, new Events.OnIntChangedEventArgs { i = currentThirst });
        }

        //a method to deplete thirst
        public void DepleteThirst(int amount)
        {
            //decreases current thirst
            SetCurrentThirst(GetCurrentThirst() - amount);

            //checking if our thirst is 0
            if (GetCurrentThirst() <= 0)
            {
                //we have to invoke a differnt method (thirstdepletedtick) that then calls the depleteHealth method
                //cos invokes cannot pass through paramaters
                if (!IsInvoking(nameof(ThirstDepletedTick)))
                {
                    InvokeRepeating(nameof(ThirstDepletedTick), 0f, thirstDepletedTickRate);
                }
            }
        }

        //a method to replenish thirst
        public void ReplenishThirst(int amount)
        {
            //increaes current thirst
            SetCurrentThirst(GetCurrentThirst() + amount);

            //if we're above 0 thirst
            if (GetCurrentThirst() > 0)
            {
                //cancel the thirst depleted tick
                CancelInvoke(nameof(ThirstDepletedTick));
            }

            if (GetCurrentThirst() >= maxThirst) //if we have full thirst
            {
                //then debug log that we have full thirst
                //Debug.Log("Character (" + gameObject.name + ") is at Full Thirst");
            }
        }

        #endregion

        #region Stamina

        //used to grab our current stamina
        //(maybe replace with events instead?)
        public int GetCurrentStamina() => currentStamina;
        public int GetMaxStamina() => maxStamina;

        //sets our current stamina
        public void SetCurrentStamina(int amount)
        {
            currentStamina = amount;

            //doing our clamping in here
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            //firing off our event here
            OnStaminaChanged?.Invoke(this, new Events.OnIntChangedEventArgs { i = currentStamina });
        }

        //method to invoke our stamina tick
        public void DepleteStamina()
        {
            InvokeRepeating(nameof(StaminaDepletionTick), 0, staminaDepletionTickRate);
        }

        //a method to deplete stamina
        public void DepleteStamina(int amount)
        {
            //decreases current stamina
            SetCurrentStamina(GetCurrentStamina() - amount);

            //checking if our stamina is 0
            if (GetCurrentStamina() <= 0)
            {
                //Debug.Log("Character (" + gameObject.name + ") Has no Stamina!");
            }
        }

        //a method to replenish stamina, calling the stamina replenish tick after a certain amount of time
        public void ReplenishStamina()
        {
            InvokeRepeating(nameof(StaminaReplenishTick), staminaReplenishTime, staminaReplenishTickRate);
        }

        //a method to replenish stamina
        public void ReplenishStamina(int amount)
        {
            //increases current stamina
            SetCurrentStamina(GetCurrentStamina() + amount);

            if (currentStamina >= maxStamina) //if we have full stamina
            {
                //then debug log that we have full stamina
                //Debug.Log("Character (" + gameObject.name + ") is at Full Stamina");
            }
        }

        #endregion

        #region Melee Stamina

        public int GetCurrentMeleeStamina() => currentMeleeStamina;
        public int GetMaxMeleeStamina() => maxMeleeStamina;

        //sets our current stamina
        public void SetCurrentMeleeStamina(int amount)
        {
            currentMeleeStamina = amount;

            //doing our clamping in here
            currentMeleeStamina = Mathf.Clamp(currentMeleeStamina, 0, maxMeleeStamina);

            //firing off our event here
            OnMeleeStaminaChanged?.Invoke(this, new Events.OnIntChangedEventArgs { i = currentMeleeStamina });
        }

        //a method to deplete stamina
        public void DepleteMeleeStamina(int amount)
        {
            //decreases current stamina
            SetCurrentMeleeStamina(GetCurrentMeleeStamina() - amount);

            //checking if our stamina is 0
            if (GetCurrentMeleeStamina() <= 0)
            {
               // Debug.Log("Character (" + gameObject.name + ") Has no Melee Stamina!");
            }
        }

        //a method to replenish stamina, calling the stamina replenish tick after a certain amount of time
        public void ReplenishMeleeStamina()
        {
            InvokeRepeating(nameof(MeleeStaminaReplenishTick), meleeStaminaReplenishTime, meleeStaminaReplenishTickRate);
        }

        //a method to replenish stamina
        public void ReplenishMeleeStamina(int amount)
        {
            //increases current stamina
            SetCurrentMeleeStamina(GetCurrentMeleeStamina() + amount);

            if (currentMeleeStamina >= maxMeleeStamina) //if we have full stamina
            {
                //then debug log that we have full stamina
                //Debug.Log("Character (" + gameObject.name + ") is at Full Melee Stamina");
            }
        }

        #endregion

        #region Temperature

        //used to grab our current temperature
        //(maybe replace with events instead?)
        public Temperature GetCurrentTemperature() => currentTemperature;

        //sets our current temperature
        public void SetCurrentTemperature(Temperature temp)
        {
            currentTemperature = temp;

            //firing off our event here
            OnTemperatureChanged?.Invoke(this, new OnTemperatureChangedEventArgs { i = currentTemperature });
        }

        #endregion

        #endregion

        #region Ticks

        private void HungerTick()
        {
            DepleteHunger(hungerDepletionRate);
        }

        private void ThirstTick()
        {
            DepleteThirst(thirstDepletionRate);
        }

        //damage ticks - when hunger and thirst are depleted
        private void HungerDepletedTick()
        {
            Damage(hungerDepletedDamage);
        }

        private void ThirstDepletedTick()
        {
            Damage(thirstDepletedDamage);
        }

        //STAMINA

        public void StaminaReplenishTick()
        {
            ReplenishStamina(staminaReplenishRate);
        }

        public void StaminaDepletionTick()
        {
            DepleteStamina(staminaDepletionRate);
        }

        public void CancelStaminaReplenishTick()
        {
            CancelInvoke(nameof(StaminaReplenishTick));
        }

        public void CancelStaminaDepletionTick()
        {
            CancelInvoke(nameof(StaminaDepletionTick));
        }

        //a method to reset the stamina replenish tick
        public void ResetStaminaReplenishTick()
        {
            CancelStaminaReplenishTick();
            StaminaReplenishTick();
        }

        //MELEE STAMINA
        public void MeleeStaminaReplenishTick()
        {
            ReplenishMeleeStamina(meleeStaminaReplenishRate);
        }

        public void CancelMeleeStaminaReplenishTick()
        {
            CancelInvoke(nameof(MeleeStaminaReplenishTick));
        }
        
        //a method to reset the melee stamina replenish tick
        public void ResetMeleeStaminaReplenishTick()
        {
            CancelMeleeStaminaReplenishTick();
            ReplenishMeleeStamina();
        }

        #endregion
    }
}