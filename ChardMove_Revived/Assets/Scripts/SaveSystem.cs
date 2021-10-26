using System;
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
        public int LastLevelIndex;
        private void InitializeArray()
        {
            CompletedLevels = new bool[LevelLoader.Instance.GetLevelCount];
            for (int i = 0; i < CompletedLevels.Length; i++)
            {
                if(i < LastLevelIndex)
                {
                    CompletedLevels[i] = true;
                }
                else
                {
                    CompletedLevels[i] = false;
                }
            }
        }
        private void AssignLastLevelIndex()
        {
                LastLevelIndex = LevelLoader.LevelIndex;
        }
        public void Serialize()
        {
            //CompletedLevels[index] = true;
            LastLevelIndex = LevelLoader.LevelIndex;

            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, LastLevelIndex);
                //formatter.Serialize(fs, CompletedLevels);
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
                AssignLastLevelIndex();
                //InitializeArray();
                return;
            }
            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                LastLevelIndex = (int)formatter.Deserialize(fs);
                //CompletedLevels = (bool[])formatter.Deserialize(fs);
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
        public int RefreshLvlIndex()
        {
            return LastLevelIndex;
        }
    }
}
