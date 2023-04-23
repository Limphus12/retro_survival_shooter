using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace com.limphus.retro_survival_shooter
{
    public static class SaveManager
    {
        //had to change because unity cannot call get_persistentDataPath from a constuctor
        //private static string SAVE_FOLDER = "";

        public static readonly string DEFAULT_FOLDER = Application.persistentDataPath;
        public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
        public static readonly string SAVE_FILE = "/save.json", WORLD_FILE = "/world.json", CHUNK_FILE = "/chunk.json", SETTINGS_FILE = "/settings.json";

        public static void Init()
        {
            //Tests if the save folder exists
            if (!Directory.Exists(SAVE_FOLDER))
            {
                //creates the save folder and assigns the directory to the SAVE_FOLDER string.
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        public static void Save(string fileLocation, string saveString)
        {
            File.WriteAllText(fileLocation, saveString);
        }

        public static string Load(string fileLocation)
        {
            //checking if the file exists.
            if (File.Exists(fileLocation))
            {
                //grabbing the data from the .txt file
                string fileString = File.ReadAllText(fileLocation);

                //returns the save string
                return fileString;
            }

            //if we cannot find the file, return null
            else return null;
        }
    }
}