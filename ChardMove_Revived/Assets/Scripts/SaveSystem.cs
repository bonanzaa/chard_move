using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ChardMove
{
    [Serializable]
    public class SaveSystem
    {
        public bool[] CompletedLevels;
        private void InitializeArray()
        {
            CompletedLevels = new bool[LevelLoader.Instance.GetLevelCount];
            for (int i = 0; i < CompletedLevels.Length; i++)
            {
                CompletedLevels[i] = false;
            }
        }
        public void Serialize(int index)
        {
            CompletedLevels[index] = true;

            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, CompletedLevels);
            }
            catch(SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        public void Deserialize()
        {
            if(!File.Exists(Application.persistentDataPath + "/SaveFile.info"))
            {
                InitializeArray();
                return;
            }
            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                CompletedLevels = (bool[])formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }
    }
}
