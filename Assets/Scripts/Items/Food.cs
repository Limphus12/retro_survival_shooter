using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Food : Consumable
    {
        [Header("Attributes - Food")]
        [Tooltip("The total amount of hunger that can be consumed from this")] [SerializeField] private int hungerAmount;

        [Space]
        [SerializeField] private WeaponSway weaponSway;
        [SerializeField] private PlayerStats playerStats;

        [SerializeField] private int remainingHungerAmount = -1;

        // Start is called before the first frame update
        void Start()
        {
            //if we haven't initialized the consume remaining, do it here.
            if (remainingConsumeAmount == -1) remainingConsumeAmount = consumeAmount;

            //if we haven't initialized the hunger remaining, do it here.
            if (remainingHungerAmount == -1) remainingHungerAmount = hungerAmount;
        }

        //Used for Initialization
        void OnEnable()
        {
            //if we haven't initialized the consume remaining, do it here.
            if (remainingConsumeAmount == -1) remainingConsumeAmount = consumeAmount;

            //if we haven't initialized the hunger remaining, do it here.
            if (remainingHungerAmount == -1) remainingHungerAmount = hungerAmount;
        }

        // Update is called once per frame
        void Update() => Inputs();

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            CheckConsume();
        }

        protected override void CheckConsume()
        {
            //if we're not consuming atm
            if (!isConsuming)
            {
                //and we are holding our right mouse button
                if (rightMouseInput && remainingConsumeAmount > 0)
                {
                    //start consuming
                    StartConsume();
                }

                //if we are holding the right mouse button but we cannot consume it
                else if (rightMouseInput && remainingConsumeAmount == 0)
                {
                    Debug.Log("We cannot consume this food, we've already ate it all!");
                }
            }

            //if we're currently consuming
            else if (isConsuming)
            {
                //if we release our right mouse input
                if (!rightMouseInput)
                {
                    //stop the invoke of the Consume method
                    CancelInvoke(nameof(Consume));

                    //and end our consuming
                    EndConsume();
                }
            }
        }

        protected override void StartConsume()
        {
            isConsuming = true;

            Aim(isConsuming);

            //invoke the consume method after the consume time
            Invoke(nameof(Consume), consumeTime);

            Debug.Log("Started Consuming");
        }

        protected override void Consume()
        {
            //check for the player stats script and increase the hunger amount
            if (playerStats)
            {
                //do some maths to figure out how much of the hunger amount is used
                //hunger usage = hunger amount / consume amount
                //gotta round it to an int since we track hunger as an int

                //if we're not on the final consume, do the maths
                if (remainingConsumeAmount > 1)
                {
                    //round to an int how much hunger we're gonna consume
                    int i = Mathf.RoundToInt(hungerAmount / consumeAmount);

                    //call the replenish hunger method on our player stats reference
                    playerStats.ReplenishHunger(i);

                    //decreases our remaining hunger amount
                    remainingHungerAmount -= i;
                }

                //if we're on our last consume, just consume the rest of it, no maths required
                else if (remainingConsumeAmount == 1)
                {
                    //call the replenish hunger method on our player stats reference
                    playerStats.ReplenishHunger(remainingHungerAmount);

                    //decreases our remaining hunger amount
                    remainingHungerAmount = 0;
                }
            }

            //decrease the remaining consume amount
            remainingConsumeAmount--;

            //call the end consume menthod
            EndConsume();

            Debug.Log("Consuming");
        }

        protected override void EndConsume()
        {
            isConsuming = false;

            Aim(isConsuming);

            Debug.Log("Ended Consuming");
        }

        private void Aim(bool b)
        {
            //if we have the weapon sway reference, call the aim method on it too
            if (weaponSway) weaponSway.Aim(b);
        }
    }
}