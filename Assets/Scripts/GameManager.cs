using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class GameManager : MonoBehaviour
    {
        private GameObject player;

        public static GameObject Player;
        public static PlayerController PlayerController;
        public static PlayerStats PlayerStats;
        public static PlayerInventory PlayerInventory;
        public static PlayerInteraction PlayerInteraction;

        public static Camera PlayerCamera;

        [Header("Time Management")] // <- heh i need me some of that
        [SerializeField] private float timeMultiplier = 1f;

        [Space]
        [SerializeField] private Gradient timeGradient;
        [SerializeField] private Gradient skyGradient, equatorGradient, groundGradient;

        private void Awake() => Init();

        [SerializeField, Range(0f, 24000f)] private float currentTime;

        private float maxTime = 24000f;

        private void Init()
        {
            if (!player) player = GameObject.FindGameObjectWithTag("Player"); Player = player;

            if (Player)
            {
                PlayerController = Player.GetComponent<PlayerController>();
                PlayerStats = Player.GetComponent<PlayerStats>();
                PlayerInventory = Player.GetComponent<PlayerInventory>();
                PlayerInteraction = Player.GetComponent<PlayerInteraction>();
            }

            PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            //increment and clamp our current time
            currentTime += Time.deltaTime * timeMultiplier; currentTime = Mathf.Clamp(currentTime, 0f, maxTime);

            //reset our current time if it reaches max time
            if (currentTime == maxTime) currentTime = 0;

            //calculate a normalized time value (between 0 and 1), and do a debug
            float normalizedTimeValue = currentTime / maxTime; //Debug.Log(currentTime + " / " + normalizedTimeValue);

            //calculate the target colour from the gradients
            Color targetFogColor = timeGradient.Evaluate(normalizedTimeValue);

            Color targetSkyColor = skyGradient.Evaluate(normalizedTimeValue);
            Color targetEquatorColor = equatorGradient.Evaluate(normalizedTimeValue);
            Color targetGroundColor = groundGradient.Evaluate(normalizedTimeValue);

            //set the fog and camera background color (the same as the fog to blend nicely)
            RenderSettings.fogColor = targetFogColor;
            PlayerCamera.backgroundColor = targetFogColor;

            //...and all of the light colors 
            RenderSettings.ambientSkyColor = targetSkyColor;
            RenderSettings.ambientEquatorColor = targetEquatorColor;
            RenderSettings.ambientGroundColor = targetGroundColor;
        }
    }
}