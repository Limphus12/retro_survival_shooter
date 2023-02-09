using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public class Test
    {
        public List<ItemData> itemDatas;
    }

    public class SaveSystem : MonoBehaviour
    {
        [Header("Player Stuff")]
        [SerializeField] private Transform playerTransform;

        [SerializeField] private PlayerStats playerStats;

        [Space]
        [SerializeField] private InventoryManager playerInventory;

        [Header("World Stuff")]
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private BiomeGenerator biomeGenerator;

        [Header("Test")]
        [SerializeField] private FirearmData firearmData;
        [SerializeField] private MeleeData meleeData;
        //[SerializeField] private SustenanceData sustenanceData;

        [Space]
        public TextMeshProUGUI debugtext;

        private void Test()
        {
            List<GameObject> items = playerInventory.GetInventoryItems();

            List<ItemData> itemDatas = new List<ItemData>();

            foreach(GameObject item in items)
            {
                ItemData data = item.GetComponent<Item>().GetItemData();

                if (data == null) return;

                itemDatas.Add(data);
            }

            Test test = new Test
            {
                itemDatas = itemDatas
            };

            string str = JsonUtility.ToJson(test, true);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Saves/" + "/Test_002.json", str);
        }

        private void Test2()
        {
            //checking if the file exists.
            if (System.IO.File.Exists(Application.persistentDataPath + "/Saves/" + "/Test_002.json"))
            {
                //grabbing the data from the .txt file
                string saveString = System.IO.File.ReadAllText(Application.persistentDataPath + "/Saves/" + "/Test_002.json");

                //create a save object
                Test saveObject = JsonUtility.FromJson<Test>(saveString);

                //setting our firearm data
                //firearmData = (FirearmData)saveObject.itemDatas[0];
                //meleeData = (MeleeData)saveObject.itemDatas[1];
                //sustenanceData = (SustenanceData)saveObject.itemDatas[2];

                //debug text - 1
                //debugtext.text = "Name: " + firearmData.itemName + ", Weight: " + firearmData.itemWeight + ", Damage: " + firearmData.damage + ", Rate of Fire: " + firearmData.rateOfFire + "...";

                //debugtext.text = "Name: " + firearmData.itemName + ", Weight: " + firearmData.itemWeight + ", Damage: " + firearmData.damage + ", Rate of Fire: " + firearmData.attackRate + "..." + "Name: " + meleeData.itemName + ", Weight: " + meleeData.itemWeight + ", Damage: " + meleeData.damage + ", Rate of Fire: " + meleeData.attackRate + "..." + "Name: " + sustenanceData.itemName + ", Weight: " + sustenanceData.itemWeight + ", Consume Amount: " + sustenanceData.useAmount + ", Consume Time: " + sustenanceData.consumeTime + "...";

                //debugtext.text = "Name: " + meleeData.itemName + ", Weight: " + meleeData.itemWeight + ", Damage: " + meleeData.damage + ", Rate of Fire: " + meleeData.rateOfFire + "..." + "Name: " + sustenanceData.itemName + ", Weight: " + sustenanceData.itemWeight + ", Consume Amount: " + sustenanceData.consumeAmount + ", Consume Time: " + sustenanceData.consumeTime + "...";

                //returns the save string
                Debug.Log(saveString);
            }
        }

        private void Awake()
        {
            //initializing the save manager
            SaveManager.Init();
        }

        private void Start()
        {
            //finding the player in the scene
            //if (!playerTransform) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            //finding the player stats in the scene
            if (!playerStats) playerStats = FindObjectOfType<PlayerStats>();
            
            //finding the player in the scene (using the player stats reference)
            if (playerStats) playerTransform = playerStats.transform;

            //finding the world generator in the scene
            if (!worldGenerator) worldGenerator = FindObjectOfType<WorldGenerator>();
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
                SaveManager.Save(json);
            }

            else Debug.LogError("Missing Critical References, cannot Save!");
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