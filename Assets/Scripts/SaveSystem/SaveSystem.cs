using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public class SettingsData
    {
        public bool fullscreen;

        public Resolution currentResolution;

        public int currentQualityLevel;

        public float currentMasterVolume, currentAmbienceVolume, currentSoundVolume, currentUIScale;
    }

    public class SaveSystem : MonoBehaviour
    {
        Transform playerTransform;

        PlayerStats playerStats;

        [Space]
        PlayerInventory playerInventory;

        private WorldGenerator worldGenerator;
        private TerrainGenerator terrainGenerator;
        private BiomeGenerator biomeGenerator;

        private FirearmData firearmData;
        private MeleeData meleeData;

        //[SerializeField] private SustenanceData sustenanceData;

        private TextMeshProUGUI debugtext;

        private void Start()
        {
            //initializing the save manager
            SaveManager.Init();

            LoadSettings();
        }

        public void SaveSettings()
        {
            SettingsData settingsData = new SettingsData
            {
                fullscreen = Settings.fullscreen,
                currentResolution = Settings.currentResolution,
                currentQualityLevel = Settings.currentQualityLevel,
                currentMasterVolume = Settings.currentMasterVolume + 80,
                currentAmbienceVolume = Settings.currentAmbienceVolume + 80,
                currentSoundVolume = Settings.currentSoundVolume + 80,
                currentUIScale = UIManager.UIScale
            };

            //creating a new save object, setting the values
            SettingsSaveObject saveObject = new SettingsSaveObject { settingsData = settingsData };

            //using json utilities to write a json file -  true for prettyPrint
            string json = JsonUtility.ToJson(saveObject, true);

            //save it to the correct folder and file, passing through our json string
            SaveManager.Save(SaveManager.DEFAULT_FOLDER + SaveManager.SETTINGS_FILE, json);
        }

        private void LoadSettings()
        {
            //asking for the save string from teh save manager
            string saveString = SaveManager.Load(SaveManager.DEFAULT_FOLDER + SaveManager.SETTINGS_FILE);

            if (saveString != null)
            {
                //creating a save object from the json/string
                SettingsSaveObject saveObject = JsonUtility.FromJson<SettingsSaveObject>(saveString);

                //grab the settings data from the save object
                SettingsData settingsData = saveObject.settingsData;

                Settings.fullscreen = settingsData.fullscreen;
                Settings.currentResolution = settingsData.currentResolution;
                Settings.currentQualityLevel = settingsData.currentQualityLevel;
                Settings.currentMasterVolume = settingsData.currentMasterVolume - 80;
                Settings.currentAmbienceVolume = settingsData.currentAmbienceVolume - 80;
                Settings.currentSoundVolume = settingsData.currentSoundVolume - 80;
                UIManager.UIScale = settingsData.currentUIScale;
            }

            else Debug.Log("No Settings Save Detected!");
        }

        private void Update()
        {
            //Inputs();
        }

        private void Inputs()
        {
            //if (Input.GetKeyDown(KeyCode.F5)) Test();

            //if (Input.GetKeyDown(KeyCode.F9)) Test2();

            if (Input.GetKeyDown(KeyCode.F5)) Save();

            if (Input.GetKeyDown(KeyCode.F9)) Load();
        }

        private void Save()
        {
            //if we have the player references and the generator references
            if (playerTransform && playerStats && playerInventory && worldGenerator && terrainGenerator && biomeGenerator)
            {
                //create a new set of player data
                PlayerData playerData = new PlayerData
                {
                    currentPosition = playerTransform.position,
                    currentRotation = playerTransform.rotation,

                    currentHealth = playerStats.GetCurrentHealth(),
                    currentHunger = playerStats.GetCurrentHunger(),
                    currentThirst = playerStats.GetCurrentThirst(),
                    currentStamina = playerStats.GetCurrentStamina(),
                    currentTempurature = playerStats.GetCurrentTemperature()
                };

                //WorldDataStruct worldData = new WorldDataStruct
                {
                    //meshData = terrainGenerator.GetMeshData()
                };

                //creating a new save object, setting the values
                SaveObject saveObject = new SaveObject { playerData = playerData };

                //using json utilities to write a json file -  true for prettyPrint
                string json = JsonUtility.ToJson(saveObject, true);

                //calling the save method on the save manager
                SaveManager.Save(SaveManager.SAVE_FOLDER + SaveManager.SAVE_FILE, json);
            }

            else Debug.LogError("Missing Critical References, cannot Save!");
        }

        private void Load()
        {
            //asking for the save string from teh save manager
            string saveString = SaveManager.Load(SaveManager.SAVE_FOLDER + SaveManager.SAVE_FILE);

            if (saveString != null)
            {
                //creating a save object from the json/string
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                //grab the player data from the save object
                PlayerData playerData = saveObject.playerData;

                //set the player position from the player data we just loaded
                if (playerTransform)
                {
                    playerTransform.SetPositionAndRotation(playerData.currentPosition, playerData.currentRotation);
                }
                    
                //sets the player stats from the player data we just loaded
                if (playerStats)
                {
                    playerStats.SetCurrentHealth(playerData.currentHealth);
                    playerStats.SetCurrentHunger(playerData.currentHunger);
                    playerStats.SetCurrentThirst(playerData.currentThirst);
                    playerStats.SetCurrentStamina(playerData.currentStamina);
                    playerStats.SetCurrentTemperature(playerData.currentTempurature);
                }

                //grab the world data from the save object

                //generate the world using the world data.
                //WorldDataStruct worldData = saveObject.worldData;

                //if (terrainGenerator)
                {
                    //terrainGenerator.GenerateMesh(worldData.meshData);
                }
            }

            else Debug.Log("No Save Detected!");
        }

        private class SettingsSaveObject
        {
            public SettingsData settingsData;
        }

        private class SaveObject
        {
            public PlayerData playerData;

            //public WorldDataStruct worldData;
        }

        [Serializable]
        private struct PlayerData
        {
            public Vector3 currentPosition;

            public Quaternion currentRotation;

            public int currentHealth, currentHunger, currentThirst, currentStamina;

            public Temperature currentTempurature;
        }
    }
}