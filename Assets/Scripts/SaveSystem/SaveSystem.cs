using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.limphus.retro_survival_shooter
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;

        [SerializeField] private WorldGenerator worldGenerator;

        private void Awake()
        {
            //initializing the save manager
            SaveManager.Init();
        }

        private void Start()
        {
            //finding the player in the scene
            if (!playerTransform) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            //finding the world generator in the scene
            if (!worldGenerator) worldGenerator = GameObject.FindObjectOfType<WorldGenerator>();
        }

        private void Update()
        {
            Inputs();
        }

        private void Inputs()
        {
            if (Input.GetKeyDown(KeyCode.F5)) Save();

            if (Input.GetKeyDown(KeyCode.F9)) Load();
        }

        private void Save()
        {
            //creating a new save object, setting the values
            //SaveObject saveObject = new SaveObject { playerPosition = playerTransform.position };

            if (playerTransform && worldGenerator)
            {
                //creating a new save object, setting the values
                SaveObject saveObject = new SaveObject { playerPosition = playerTransform.position, currentChunk = worldGenerator.GetCurrentChunk() };

                //using json utilities to write a json file
                string json = JsonUtility.ToJson(saveObject);

                //calling the save method on the save manager
                SaveManager.Save(json);
            }

            else
            {
                Debug.Log("Missing World Generator, cannot Save!");
            }
        }

        private void Load()
        {
            //asking for the save string from teh save manager
            string saveString = SaveManager.Load();

            if (saveString != null)
            {
                //creating a save object from the json/string
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                //set the player position from the save object we just created.
                if (playerTransform) playerTransform.position = saveObject.playerPosition;

                //set the world generator's current chunk from the save object we created.
                if (worldGenerator) worldGenerator.SetCurrentChunk(saveObject.currentChunk);
            }

            else Debug.Log("No Save Detected!");
        }

        private class SaveObject
        {
            public Vector3 playerPosition;

            public Vector2Int currentChunk;

            //later on add health values, current game level etc.
        }
    }
}