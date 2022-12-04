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

        public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
        public static readonly string SAVE_FILE = "/save.json", WORLD_FILE = "/world.json", CHUNK_FILE = "/chunk.json";

        public static void Init()
        {
            //Tests if the save folder exists
            if (!Directory.Exists(SAVE_FOLDER))
            {
                //creates the save folder and assigns the directory to the SAVE_FOLDER string.
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        public static void Save(string saveString)
        {
            File.WriteAllText(SAVE_FOLDER + SAVE_FILE, saveString);
        }

        public static string Load()
        {
            //checking if the file exists.
            if (File.Exists(SAVE_FOLDER + SAVE_FILE))
            {
                //grabbing the data from the .txt file
                string saveString = File.ReadAllText(SAVE_FOLDER + SAVE_FILE);

                //returns the save string
                return saveString;
            }

            //if we cannot find the file, return null
            else return null;
        }
    }
}