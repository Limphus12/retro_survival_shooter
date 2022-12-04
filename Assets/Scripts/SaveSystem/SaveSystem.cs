using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public class GameObjectCollection
    {
        public List<GameObject> gameObjects = new List<GameObject>();
    }

    public class SaveSystem : MonoBehaviour
    {
        [Header("Player Stuff")]
        [SerializeField] private Transform playerTransform;

        [SerializeField] private PlayerStats playerStats;

        [Header("World Stuff")]
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private BiomeGenerator biomeGenerator;

        [Header("Test")]
        [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();

        private void Test()
        {
            GameObjectCollection gameObjCollection = new GameObjectCollection
            {
                gameObjects = gameObjects
            };

            string potion = JsonUtility.ToJson(gameObjCollection, true);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Saves/" + "/GameObjects.json", potion);
        }

        private void Awake()
        {
            //initializing the save manager
            SaveManager.Init();
        }

        private void Start()
        {
            //finding the player in the scene
            if (!playerTransform) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            //finding the player stats in the scene
            if (!playerStats) playerStats = GameObject.FindObjectOfType<PlayerStats>();

            //finding the world generator in the scene
            if (!worldGenerator) worldGenerator = GameObject.FindObjectOfType<WorldGenerator>();
        }

        private void Update()
        {
            Inputs();
        }

        private void Inputs()
        {
            if (Input.GetKeyDown(KeyCode.F5)) Test();
            //if (Input.GetKeyDown(KeyCode.F5)) Save();

            //if (Input.GetKeyDown(KeyCode.F9)) Load();
        }

        

        private void Save()
        {
            //creating a new save object, setting the values
            //SaveObject saveObject = new SaveObject { playerPosition = playerTransform.position };

            //if we have the player transform, the player stats and the world generator
            if (playerTransform && playerStats && worldGenerator)
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

                //creating a new save object, setting the values
                SaveObject saveObject = new SaveObject { playerData = playerData, currentChunk = worldGenerator.GetCurrentChunk() };

                //using json utilities to write a json file -  true for prettyPrint
                string json = JsonUtility.ToJson(saveObject, true);

                //calling the save method on the save manager
                SaveManager.Save(json);
            }

            else Debug.Log("Missing Critical References, cannot Save!");
        }

        private void Load()
        {
            //asking for the save string from teh save manager
            string saveString = SaveManager.Load();

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

                //set the world generator's current chunk from the save object we created.
                if (worldGenerator) worldGenerator.SetCurrentChunk(saveObject.currentChunk);
            }

            else Debug.Log("No Save Detected!");
        }

        private class SaveObject
        {
            public PlayerData playerData;

            public Vector2Int currentChunk;
        }

        [Serializable]
        private struct PlayerData
        {
            public Vector3 currentPosition;

            public Quaternion currentRotation;

            public int currentHealth, currentHunger, currentThirst, currentStamina;

            public Temperature currentTempurature;
        }

        [Serializable]
        private struct WorldData
        {
            public int seed;

            public Vector2Int currentChunk;
        }

        [Serializable]
        private struct ChunkData
        {
            public Vector2Int chunk;

            GameObject[] gameObjects;

            //idk what to put here lmao
        }

    }
}