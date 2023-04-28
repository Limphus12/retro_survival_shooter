using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct WeatherDataStruct
    {
        public string name;

        [Space]
        public Gradient timeGradient;
        public Gradient skyGradient, equatorGradient, groundGradient;

        [Space]
        public ParticleSystem particles;
        public float emmisionRate;

        [Space]
        public float fogDistance;

        [Space]
        public AudioSource weatherSound;
        public float volume;

        //public Volume postProcessingVolume; ADD LATER, AFTER DEADLINE!
    }

    public class GameManager : MonoBehaviour
    {
        private GameObject player;

        public static GameObject Player;
        public static PlayerController PlayerController;
        public static PlayerStats PlayerStats;
        public static PlayerInventory PlayerInventory;
        public static PlayerInteraction PlayerInteraction;
        public static Vector3 PlayerPosition;

        public static Camera PlayerCamera;

        [Header("Time Management")] // <- heh i need me some of that
        [SerializeField] private float timeMultiplier = 1f;
        [SerializeField, Range(0f, 24000f)] private float currentTime;

        [Header("Sky & Weather")]
        [SerializeField] private float weatherLerpMultiplier = 0.1f;

        [SerializeField] private Vector2 weatherChangeIntervalRange = new Vector2(4000, 24000);

        [Space]
        [SerializeField] private WeatherDataStruct[] weathers;

        private float maxTime = 24000f;

        private float currentWeatherChangeInterval, weatherTime;

        private UnityEngine.Gradient timeGradient;
        private UnityEngine.Gradient skyGradient, equatorGradient, groundGradient;

        private float fogDistance;

        bool isChangingWeather;

        private WeatherDataStruct currentWeather, oldWeather;

        private void Awake() => Init();

        private void Init()
        {
            if (!player) player = GameObject.FindGameObjectWithTag("Player"); Player = player;

            if (Player)
            {
                PlayerController = Player.GetComponent<PlayerController>();
                PlayerStats = Player.GetComponent<PlayerStats>();
                PlayerInventory = Player.GetComponent<PlayerInventory>();
                PlayerInteraction = Player.GetComponent<PlayerInteraction>();
                PlayerPosition = Player.transform.position;
            }

            PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            currentWeather = weathers[Random.Range(0, weathers.Length)];

            //how to access the post processing volume
            //currentWeather.postProcessingVolume.profile.

            //setting our ammo to half full for now...
            PlayerAmmo.SetAmmo(AmmoType.PISTOL, PlayerAmmo.GetMaxAmmo(AmmoType.PISTOL) / 2);
        }

        private void Start() => ChangeWeather();

        private void Update()
        {
            if (Player) PlayerPosition = Player.transform.position;

            if (isChangingWeather) LerpWeather();

            UpdateTime(); UpdateWeather();
        }

        private void UpdateTime()
        {
            //increment and clamp our current time
            currentTime += Time.deltaTime * timeMultiplier; currentTime = Mathf.Clamp(currentTime, 0f, maxTime);
            weatherTime += Time.deltaTime * timeMultiplier;

            if (weatherTime >= currentWeatherChangeInterval) ChangeWeather();

            //reset our current time if it reaches max time
            if (currentTime == maxTime) currentTime = 0;
        }

        private void UpdateWeather()
        {
            //calculate a normalized time value (between 0 and 1), and do a debug
            float normalizedTimeValue = currentTime / maxTime; //Debug.Log(currentTime + " / " + normalizedTimeValue);

            //calculate the target colour from the gradients
            Color targetFogColor = timeGradient.Evaluate(normalizedTimeValue);

            Color targetSkyColor = skyGradient.Evaluate(normalizedTimeValue);
            Color targetEquatorColor = equatorGradient.Evaluate(normalizedTimeValue);
            Color targetGroundColor = groundGradient.Evaluate(normalizedTimeValue);

            //set the fog and camera background color (the same as the fog to blend nicely)
            RenderSettings.fogColor = targetFogColor;
            RenderSettings.fogEndDistance = fogDistance;
            PlayerCamera.backgroundColor = targetFogColor;

            //...and all of the light colors
            RenderSettings.ambientSkyColor = targetSkyColor;
            RenderSettings.ambientEquatorColor = targetEquatorColor;
            RenderSettings.ambientGroundColor = targetGroundColor;
        }

        private void ChangeWeather()
        {
            isChangingWeather = true; weatherTime = 0f;

            currentWeatherChangeInterval = Random.Range(weatherChangeIntervalRange.x, weatherChangeIntervalRange.y);

            oldWeather = currentWeather;

            //currentWeather = weathers[3]; return;

            //while loop to ensure we pick a different weather
            while (currentWeather.Equals(oldWeather))
            {
                currentWeather = weathers[Random.Range(0, weathers.Length)];
            }
        }

        private float lerpI = 0f;

        private void LerpWeather()
        {
            lerpI += Time.deltaTime * weatherLerpMultiplier;

            //gradients
            timeGradient = utilities.Gradient.LerpNoAlpha(oldWeather.timeGradient, currentWeather.timeGradient, lerpI);
            skyGradient = utilities.Gradient.LerpNoAlpha(oldWeather.skyGradient, currentWeather.skyGradient, lerpI);
            equatorGradient = utilities.Gradient.LerpNoAlpha(oldWeather.equatorGradient, currentWeather.equatorGradient, lerpI);
            groundGradient = utilities.Gradient.LerpNoAlpha(oldWeather.groundGradient, currentWeather.groundGradient, lerpI);

            //fog
            fogDistance = Mathf.Lerp(oldWeather.fogDistance, currentWeather.fogDistance, lerpI);

            //weather particles
            if (oldWeather.particles != null)
            {
                ParticleSystem.EmissionModule emissionModule = oldWeather.particles.emission;

                emissionModule.rateOverTime = Mathf.Lerp(oldWeather.emmisionRate, 0, lerpI);
            }

            if (currentWeather.particles != null)
            {
                ParticleSystem.EmissionModule emissionModule = currentWeather.particles.emission;

                emissionModule.rateOverTime = Mathf.Lerp(0, currentWeather.emmisionRate, lerpI);
            }

            //weather sounds
            if (oldWeather.weatherSound != null) oldWeather.weatherSound.volume = Mathf.Lerp(oldWeather.volume, 0, lerpI);
            if (currentWeather.weatherSound != null) currentWeather.weatherSound.volume = Mathf.Lerp(0, currentWeather.volume, lerpI);

            if (lerpI >= 1f)
            {
                isChangingWeather = false;
                lerpI = 0f;
            }
        }
    }
}